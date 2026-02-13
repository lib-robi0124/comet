using Comet.Services.Interfaces;
using Comet.ViewModels.Auction;
using Comet.ViewModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Comet.Controllers
{
    [AllowAnonymous]
    public class AuctionController : Controller
    {
        private readonly IProductService _productService;
        private readonly IAuctionService _auctionService;
        private readonly ILogger<AuctionController> _logger;

        public AuctionController(
            IProductService productService,
            IAuctionService auctionService,
            ILogger<AuctionController> logger)
        {
            _productService = productService;
            _auctionService = auctionService;
            _logger = logger;
        }

        // GET: Auction Dashboard
        [HttpGet]
        // GET: Auction Dashboard
        [HttpGet]
        public async Task<IActionResult> Index(string search = "", string category = "")
        {
            try
            {
                var model = new AuctionDashboardVM
                {
                    SearchTerm = search,
                    SelectedCategory = category
                };

                // Get published products from your existing ProductService
                var products = await _productService.GetPublishedProductsAsync();

                // Get current user ID if authenticated
                int? currentUserId = GetCurrentUserId();

                // Map products to AuctionProductVM - CORRECTED based on your ProductVM
                model.Products = products.Select(p => new AuctionProductVM
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode ?? string.Empty,
                    Grade = p.Grade ?? string.Empty,
                    ProductCategory = p.ProductCategory.ToString(), // Enum to string
                    StartingPrice = p.Price, // Price is the starting price
                    CurrentHighestBid = p.CurrentHighestBid, // This exists in your VM
                    // Product Specifications - ALL properties that AuctionProductVM needs
                    ColorTopSide = p.ColorTopSide ?? string.Empty,
                    ColorBottomSide = p.ColorBottomSide ?? string.Empty,
                    ZincCoating = p.ZincCoating ?? string.Empty,
                    Thickness = p.Thickness,
                    Width = p.Width,
                    GrossWeight = p.GrossWeight,
                    NetWeight = p.NetWeight,
                    Defects = p.Defects ?? string.Empty,
                    AuctionEndDate = p.PublishedAt?.AddDays(7) ?? DateTime.Now.AddDays(7), // Use PublishedAt + 7 days
                    HasBids = p.CurrentHighestBid > 0, // If there's a bid, it's > 0
                    BidCount = 0 // You'll need to get this from a separate query or add to ProductVM
                }).Where(p => p.IsAuctionActive).ToList();

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    model.Products = model.Products.Where(p =>
                        p.ProductCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Grade.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Get categories for filtering
                model.Categories = products
                    .GroupBy(p => p.ProductCategory.ToString())
                    .Select(g => new CategoryFilterVM
                    {
                        Name = g.Key,
                        Count = g.Count(),
                        IsSelected = g.Key == category
                    })
                    .ToList();

                // Apply category filter
                if (!string.IsNullOrEmpty(category))
                {
                    model.Products = model.Products.Where(p => p.ProductCategory == category).ToList();
                }

                // Calculate stats for logged-in users
                if (currentUserId.HasValue)
                {
                    var userBids = await _auctionService.GetUserBidsAsync(currentUserId.Value);
                    model.MyRecentBids = userBids.Take(5).ToList();
                    model.Stats.TotalMyBids = userBids.Count;
                    model.Stats.WinningBids = userBids.Count(b => b.IsWinning);
                    model.Stats.TotalBidValue = userBids.Sum(b => b.MyBidAmount);
                }

                model.Stats.TotalActiveAuctions = model.Products.Count;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading auction dashboard");
                TempData["ErrorMessage"] = "Error loading products. Please try again.";
                return View(new AuctionDashboardVM());
            }
        }

        // GET: Auction/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var productDetails = await _productService.GetProductDetailsAsync(id);
                if (productDetails == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction("Index");
                }
                int? currentUserId = GetCurrentUserId();
                // Get bid history for this product
                var bidHistory = await _auctionService.GetBidHistoryAsync(id, currentUserId);
                // Get current highest bid
                var currentHighestBid = await _auctionService.GetCurrentHighestBidAsync(id);
                var viewModel = new ProductDetailsVM
                {
                    // Basic Info
                    Id = productDetails.Id,
                    ProductCode = productDetails.ProductCode ?? string.Empty,
                    Grade = productDetails.Grade ?? string.Empty,
                    ProductCategory = productDetails.ProductCategory.ToString(), // Enum to string

                    // Pricing
                    StartingPrice = productDetails.Price, // Price is the starting price
                    CurrentHighestBid = currentHighestBid ?? productDetails.CurrentHighestBid,

                    // Product Specifications - ALL from your ProductVM
                    ColorTopSide = productDetails.ColorTopSide ?? string.Empty,
                    ColorBottomSide = productDetails.ColorBottomSide ?? string.Empty,
                    ZincCoating = productDetails.ZincCoating ?? string.Empty,
                    Thickness = productDetails.Thickness,
                    Width = productDetails.Width,
                    GrossWeight = productDetails.GrossWeight,
                    NetWeight = productDetails.NetWeight,
                    Defects = productDetails.Defects ?? string.Empty,

                    // Auction Info
                    AuctionEndDate = (productDetails.PublishedAt == default(DateTime) ? DateTime.Now : productDetails.PublishedAt).AddDays(7),
                    HasBids = bidHistory.Any(),
                    BidCount = bidHistory.Count,
                    BidHistory = bidHistory,

                    // Note: Quantity and UnitOfMeasure are calculated properties in the VM
                    // so we don't need to set them here
                };

                var bidVM = new BidVM
                {
                    ProductId = id,
                    ProductCode = productDetails.ProductCode ?? string.Empty,
                    MinimumAllowedBid = viewModel.MinimumNextBid
                };

                // Pre-fill company name if user is logged in
                if (currentUserId.HasValue)
                {
                    // You might want to get the actual company name from user claims or database
                    bidVM.CompanyName = User.Identity?.Name ?? string.Empty;
                    bidVM.UserId = currentUserId.Value;

                    // Alternative: If you have company name stored in claims
                    var companyNameClaim = User.FindFirstValue("CompanyName");
                    if (!string.IsNullOrEmpty(companyNameClaim))
                    {
                        bidVM.CompanyName = companyNameClaim;
                    }
                }
                ViewBag.BidViewModel = bidVM;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product details {ProductId}", id);
                TempData["ErrorMessage"] = "Error loading product details.";
                return RedirectToAction("Index");
            }
        }

        // POST: Auction/PlaceBid - Now super clean!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBid(BidVM bidViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors below.";
                return RedirectToAction("Details", new { id = bidViewModel.ProductId });
            }

            try
            {
                // Set bid time
                bidViewModel.BidTime = DateTime.Now;

                // Add user info if logged in
                var userId = GetCurrentUserId();
                if (userId.HasValue)
                {
                    bidViewModel.UserId = userId.Value;
                }
                // Validate bid amount against current highest
                var currentHighestBid = await _auctionService.GetCurrentHighestBidAsync(bidViewModel.ProductId);
                var isValid = await _auctionService.ValidateBidAsync(
                    bidViewModel.ProductId,
                    bidViewModel.Amount,
                    currentHighestBid
                );

                if (!isValid)
                {
                    var minBid = currentHighestBid ??
                                (await _productService.GetProductDetailsAsync(bidViewModel.ProductId))?.StartingPrice ?? 0;
                    TempData["ErrorMessage"] = $"Bid must be higher than current bid: {minBid:C}";
                    return RedirectToAction("Details", new { id = bidViewModel.ProductId });
                }

                // Let the service handle all the complex logic
                var result = await _auctionService.PlaceBidAsync(bidViewModel);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bid for product {ProductId}", bidViewModel.ProductId);
                TempData["ErrorMessage"] = "Error placing bid. Please try again.";
            }

            return RedirectToAction("Details", new { id = bidViewModel.ProductId });
        }

        // GET: Auction/MyBids
        [HttpGet]
        [Authorize] // Only authenticated users can see their bids
        public async Task<IActionResult> MyBids()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue)
                {
                    return Challenge(); // This will trigger login
                }

                var myBids = await _auctionService.GetUserBidsAsync(currentUserId.Value);
                // Get statistics for each bid's product
                foreach (var bid in myBids)
                {
                    bid.CurrentHighestBid = await _auctionService.GetCurrentHighestBidAsync(bid.ProductId) ?? 0;
                }
                return View(myBids);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user bids");
                TempData["ErrorMessage"] = "Error loading your bids.";
                return RedirectToAction("Index");
            }
        }

        // GET: Auction/ProductBids/5 - Optional: View all bids for a product
        [HttpGet]
        public async Task<IActionResult> ProductBids(int id)
        {
            try
            {
                var product = await _productService.GetProductDetailsAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var bids = await _auctionService.GetProductBidsAsync(id);
                var statistics = await _auctionService.GetProductBidStatisticsAsync(id);
                var currentUserId = GetCurrentUserId();
                ViewBag.ProductCode = product.ProductCode;
                ViewBag.Statistics = statistics;
                ViewBag.CurrentUserId = currentUserId;
                return View(bids);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bids for product {ProductId}", id);
                TempData["ErrorMessage"] = "Error loading bids.";
                return RedirectToAction("Index");
            }
        }
        // GET: Auction/CheckBidStatus/5
        [HttpGet]
        public async Task<IActionResult> CheckBidStatus(int id)
        {
            try
            {
                var currentHighestBid = await _auctionService.GetCurrentHighestBidAsync(id);
                var statistics = await _auctionService.GetProductBidStatisticsAsync(id);

                return Json(new
                {
                    success = true,
                    currentHighestBid = currentHighestBid?.ToString("C"),
                    totalBids = statistics.TotalBids,
                    lastBidTime = statistics.LastBidTime?.ToString("HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking bid status for product {ProductId}", id);
                return Json(new { success = false, message = "Error checking bid status" });
            }
        }
        #region Helper Methods
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
        #endregion
    }
}


