using Comet.DataAccess.Interfaces;
using Comet.Services.Interfaces;
using Comet.ViewModels.Admin;
using Comet.ViewModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourProjectName.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<AdminController> _logger;
        private readonly IUserService _userService;

        public AdminController(
            IProductService productService,
            IProductRepository productRepository,
            ILogger<AdminController> logger,
            IUserService userService)
        {
            _productService = productService;
            _productRepository = productRepository;
            _logger = logger;
            _userService = userService;
        }

        // GET: Admin
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardVM = new AdminDashboardVM
                {
                    TotalProducts = await _productRepository.GetCountAsync(),
                    PublishedProducts = (await _productRepository.GetPublishedProductsAsync()).Count(),
                    RecentUploads = await GetRecentImportsAsync(), // You'd implement this
                    SystemHealth = GetSystemHealth() // Optional
                };

                return View(dashboardVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["ErrorMessage"] = "Error loading dashboard.";
                return View(new AdminDashboardVM());
            }
        }

        #region Product Management

        // GET: Admin/Products
        [HttpGet]
        public async Task<IActionResult> Products(string search = "", int page = 1, int pageSize = 20)
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p =>
                        p.ProductCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Grade.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                // Pagination
                var totalItems = products.Count();
                var pagedProducts = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.SearchTerm = search;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                ViewBag.PageSize = pageSize;

                return View(pagedProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products for admin");
                TempData["ErrorMessage"] = "Error loading products.";
                return View(new List<ProductVM>());
            }
        }

        // GET: Admin/ImportProducts
        [HttpGet]
        public IActionResult ImportProducts()
        {
            var viewModel = new AdminProductImportVM
            {
                LastImportDate = GetLastImportDate(), // You'd implement this
                ImportHistory = GetImportHistory() // You'd implement this
            };
            return View(viewModel);
        }

        // POST: Admin/ImportProducts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportProducts(AdminProductImportVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                using var stream = viewModel.ExcelFile.OpenReadStream();

                // Log import attempt
                _logger.LogInformation("Admin {User} started product import at {Time}",
                    User.Identity?.Name, DateTime.Now);

                var importResult = await _productService.ImportFromExcelAsync(
                    stream,
                    viewModel.OverwriteExisting);

                // Store import result in TempData for display
                TempData["ImportResult"] = System.Text.Json.JsonSerializer.Serialize(importResult);
                TempData["ImportFileName"] = viewModel.ExcelFile.FileName;
                TempData["ImportTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (importResult.Success && importResult.SuccessfullyImported > 0)
                {
                    TempData["SuccessMessage"] =
                        $"✅ Successfully imported {importResult.SuccessfullyImported} products. " +
                        $"Failed: {importResult.FailedRows}";

                    _logger.LogInformation("Admin {User} successfully imported {Count} products",
                        User.Identity?.Name, importResult.SuccessfullyImported);
                }
                else if (importResult.FailedRows > 0)
                {
                    TempData["WarningMessage"] =
                        $"⚠️ Imported {importResult.SuccessfullyImported} products with {importResult.FailedRows} errors. " +
                        $"Check the error details below.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "❌ No products were imported. Please check the file format and try again.";
                }

                // Redirect to result page to show detailed results
                return RedirectToAction("ImportResults");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing products by admin {User}", User.Identity?.Name);
                ModelState.AddModelError("", "Error importing products. Please check the file format and try again.");

                viewModel.Result = new ImportResult
                {
                    Success = false,
                    Errors = new List<ImportError>
                    {
                        new ImportError
                        {
                            RowNumber = 0,
                            ErrorMessage = $"Import failed: {ex.Message}"
                        }
                    }
                };

                return View(viewModel);
            }
        }

        // GET: Admin/ImportResults
        [HttpGet]
        public IActionResult ImportResults()
        {
            var viewModel = new ImportResultsVM();

            if (TempData["ImportResult"] != null)
            {
                viewModel.Result = System.Text.Json.JsonSerializer.Deserialize<ImportResult>(
                    TempData["ImportResult"].ToString() ?? "{}");
                viewModel.FileName = TempData["ImportFileName"]?.ToString() ?? "Unknown";
                viewModel.ImportTime = TempData["ImportTime"]?.ToString() ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return View(viewModel);
        }

        // GET: Admin/DownloadTemplate
        [HttpGet]
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                var stream = await _productService.GenerateTemplateAsync();
                _logger.LogInformation("Admin {User} downloaded product template", User.Identity?.Name);

                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Product_Template_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating template by admin {User}", User.Identity?.Name);
                TempData["ErrorMessage"] = "Error generating template file.";
                return RedirectToAction("ImportProducts");
            }
        }

        #endregion

        #region Bulk Operations

        // POST: Admin/BulkPublish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkPublish([FromBody] List<int> productIds)
        {
            try
            {
                int successCount = 0;
                foreach (var id in productIds)
                {
                    if (await _productService.PublishProductAsync(id))
                        successCount++;
                }

                _logger.LogInformation("Admin {User} bulk published {Count} products",
                    User.Identity?.Name, successCount);

                return Json(new { success = true, count = successCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk publish by admin {User}", User.Identity?.Name);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/BulkUnpublish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUnpublish([FromBody] List<int> productIds)
        {
            try
            {
                int successCount = 0;
                foreach (var id in productIds)
                {
                    if (await _productService.UnpublishProductAsync(id))
                        successCount++;
                }

                _logger.LogInformation("Admin {User} bulk unpublished {Count} products",
                    User.Identity?.Name, successCount);

                return Json(new { success = true, count = successCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk unpublish by admin {User}", User.Identity?.Name);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/BulkDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> productIds)
        {
            try
            {
                // Implement bulk delete in your repository
                // await _productRepository.BulkDeleteAsync(productIds);

                _logger.LogInformation("Admin {User} bulk deleted {Count} products",
                    User.Identity?.Name, productIds.Count);

                return Json(new { success = true, count = productIds.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk delete by admin {User}", User.Identity?.Name);
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<List<ImportHistoryVM>> GetRecentImportsAsync(int count = 5)
        {
            // You'd implement this from a ImportLog table
            return new List<ImportHistoryVM>();
        }

        private DateTime? GetLastImportDate()
        {
            // You'd implement this from a ImportLog table
            return null;
        }

        private List<ImportHistoryVM> GetImportHistory()
        {
            // You'd implement this from a ImportLog table
            return new List<ImportHistoryVM>();
        }

        private SystemHealthVM GetSystemHealth()
        {
            return new SystemHealthVM
            {
                Status = "Healthy",
                LastChecked = DateTime.Now,
                TotalProducts = _productRepository.GetCountAsync().Result // Better to make method async
            };
        }

        #endregion
    }
}