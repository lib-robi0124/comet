using Comet.ViewModels.Models;

namespace Comet.Services.Interfaces
{
    public interface IProductService
    {
        Task<ImportResult> ImportFromExcelAsync(Stream excelStream, bool overwriteExisting);
        Task<IEnumerable<ProductVM>> GetAllProductsAsync();
        Task<ProductVM?> GetProductByIdAsync(int id);
        Task<bool> UpdateProductAsync(ProductVM viewModel);
        Task<bool> PublishProductAsync(int id);
        Task<bool> UnpublishProductAsync(int id);
        Task<IEnumerable<ProductVM>> GetPublishedProductsAsync();
        Task<ProductDetailsVM> GetProductDetailsAsync(int id);
        Task<Stream> GenerateTemplateAsync();
    }
}
