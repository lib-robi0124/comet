using Comet.DataAccess.Interfaces;
using Comet.Services.Interfaces;
using Comet.ViewModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        // GET: Auction
        [HttpGet]
        public async Task<IActionResult> Index(string search = "")
        {
            try
            {
                var products = await _productService.GetPublishedProductsAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p =>
                        p.ProductCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Grade.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.ProductCategory.ToString().Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                ViewBag.SearchTerm = search;
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading auction products");
                TempData["ErrorMessage"] = "Error loading products.";
                return View(new List<ProductVM>());
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

                ViewBag.BidViewModel = new BidVM { ProductId = id };
                return View(productDetails);
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product details {ProductId}", id);
                TempData["ErrorMessage"] = "Error loading product details.";
                return RedirectToAction("Index");
            }
        }
        // POST: Auction/PlaceBid
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
                _logger.LogError(ex, "Error placing bid");
                TempData["ErrorMessage"] = "Error placing bid. Please try again.";
            }

            return RedirectToAction("Details", new { id = bidViewModel.ProductId });
        }
    }

}
