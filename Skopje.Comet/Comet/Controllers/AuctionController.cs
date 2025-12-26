using Comet.DataAccess.Interfaces;
using Comet.Services.Interfaces;
using Comet.ViewModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace Comet.Controllers
{
    public class AuctionController : Controller
    {
        private readonly IAuctionService _auctionService;
        private readonly IProductRepository _repo;

        public AuctionController(IAuctionService auctionService, IProductRepository repo)
        {
            _auctionService = auctionService;
            _repo = repo;
        }

        public async Task<IActionResult> Products()
        {
            var products = (await _repo.GetAllAsync())
                .Select(p => new ProductListVM
                {
                    ProductId = p.Id,
                    ProductCode = p.ProductCode,
                    Category = p.ProductCategory.ToString(),
                    MinPrice = p.Price
                }).ToList();

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBid(PlaceBidVM vm)
        {
            await _auctionService.PlaceBidAsync(vm.ProductId, vm.OfferedPrice);
            return RedirectToAction("Products");
        }
    }

}
