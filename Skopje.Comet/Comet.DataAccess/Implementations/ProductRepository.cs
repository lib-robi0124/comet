using Comet.DataAccess.DataContext;
using Comet.DataAccess.Interfaces;
using Comet.Domain.Entities;
using Comet.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Comet.DataAccess.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task BulkInsertOrUpdateAsync(IEnumerable<Product> products)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var product in products)
                {
                    var existing = await _context.Products
                        .FirstOrDefaultAsync(p => p.ProductCode == product.ProductCode);

                    if (existing != null)
                    {
                        // Update existing
                        _context.Entry(existing).CurrentValues.SetValues(product);
                        existing.Id = existing.Id; // Preserve ID
                    }
                    else
                    {
                        // Insert new
                        await _context.Products.AddAsync(product);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _context.Products.AnyAsync(p => p.Id == id);
        public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();
        public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category)
        {
            return await _context.Products
            .Where(p => p.ProductCategory == category)
            .ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id) => await _context.Products.FindAsync(id);
        public async Task<Product> GetByProductCodeAsync(string productCode) => await _context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == productCode);
        public async Task<int> GetCountAsync() => await _context.Products.CountAsync();
        public async Task<bool> ProductCodeExistsAsync(string productCode) => await _context.Products
                .AnyAsync(p => p.ProductCode == productCode);
        public async Task UpdateAsync(Product entity)
        {
            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
