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
        Task<int> GetCountAsync();
    }
}
