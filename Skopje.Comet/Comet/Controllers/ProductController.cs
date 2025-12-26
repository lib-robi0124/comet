using Comet.DataAccess.Interfaces;
using Comet.Services.Interfaces;
using Comet.ViewModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace Comet.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductImportService _importService;
        private readonly IProductRepository _repo;

        public ProductController(IProductImportService importService, IProductRepository repo)
        {
            _importService = importService;
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Import() => View(new UploadExcelViewModel());

        [HttpPost]
        public async Task<IActionResult> Import(UploadExcelViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            using var stream = vm.ExcelFile.OpenReadStream();
            vm.Result = await _importService.ImportFromExcelAsync(stream, vm.OverwriteExisting);

            return View(vm);
        }

        public async Task<IActionResult> Index()
        {
            var products = await _repo.GetAllAsync();
            return View(products);
        }
    }

}
