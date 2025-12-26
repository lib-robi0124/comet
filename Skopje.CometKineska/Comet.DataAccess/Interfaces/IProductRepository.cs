using Comet.Domain.Entities;
using Comet.Domain.Enums;

namespace Comet.DataAccess.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetByProductCodeAsync(string productCode);
        Task<bool> ProductCodeExistsAsync(string productCode);
        Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category);
        Task BulkInsertOrUpdateAsync(IEnumerable<Product> products);
        Task<IEnumerable<Product>> GetPublishedProductsAsync();
        Task<int> GetCountAsync();
        Task SaveChangesAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task PublishProductAsync(int productId);
        Task UnpublishProductAsync(int productId);
    }
}
