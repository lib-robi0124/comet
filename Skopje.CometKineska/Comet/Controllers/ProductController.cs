using Comet.DataAccess.Interfaces;
using Comet.Services.Interfaces;
using Comet.ViewModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comet.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: Product/Import
        [HttpGet]
        public IActionResult Import()
        {
            return View(new UploadExcelVM());
        }

        // POST: Product/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(UploadExcelVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                using var stream = viewModel.ExcelFile.OpenReadStream();
                var importResult = await _productService.ImportFromExcelAsync(
                    stream,
                    viewModel.OverwriteExisting);

                viewModel.Result = importResult;

                if (importResult.Success && importResult.SuccessfullyImported > 0)
                {
                    TempData["SuccessMessage"] =
                        $"Successfully imported {importResult.SuccessfullyImported} products.";
                }
                else if (importResult.FailedRows > 0)
                {
                    TempData["WarningMessage"] =
                        $"Imported {importResult.SuccessfullyImported} products with {importResult.FailedRows} errors.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing products");
                ModelState.AddModelError("", "Error importing products. Please check the file format.");
                viewModel.Result = new ImportResult
                {
                    Success = false,
                    Errors = { new ImportError { ErrorMessage = ex.Message } }
                };
            }

            return View(viewModel);
        }

        // GET: Product/DownloadTemplate
        [HttpGet]
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                var stream = await _productService.GenerateTemplateAsync();
                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Product_Template.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating template");
                TempData["ErrorMessage"] = "Error generating template file.";
                return RedirectToAction("Import");
            }
        }

        // GET: Product
        [HttpGet]
        public async Task<IActionResult> Index(string search = "")
        {
            try
            {
                IEnumerable<ProductVM> products;
                if (string.IsNullOrEmpty(search))
                {
                    products = await _productService.GetAllProductsAsync();
                }
                else
                {
                    // In a real app, you'd implement search in the service
                    var allProducts = await _productService.GetAllProductsAsync();
                    products = allProducts.Where(p =>
                        p.ProductCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Grade.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.ColorTopSide.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                ViewBag.SearchTerm = search;
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                TempData["ErrorMessage"] = "Error loading products.";
                return View(new List<ProductVM>());
            }
        }

        // GET: Product/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product {ProductId}", id);
                TempData["ErrorMessage"] = "Error loading product.";
                return RedirectToAction("Index");
            }
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var success = await _productService.UpdateProductAsync(viewModel);
                if (success)
                {
                    TempData["SuccessMessage"] = "Product updated successfully.";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", viewModel.Id);
                ModelState.AddModelError("", "Error updating product. Please try again.");
                return View(viewModel);
            }
        }

        // POST: Product/Publish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            try
            {
                var success = await _productService.PublishProductAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Product published successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error publishing product.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing product {ProductId}", id);
                TempData["ErrorMessage"] = "Error publishing product.";
            }

            return RedirectToAction("Index");
        }

        // POST: Product/Unpublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(int id)
        {
            try
            {
                var success = await _productService.UnpublishProductAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Product unpublished successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error unpublishing product.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing product {ProductId}", id);
                TempData["ErrorMessage"] = "Error unpublishing product.";
            }

            return RedirectToAction("Index");
        }
    }

}
