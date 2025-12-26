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
            foreach (var product in products)
            {
                var existing = await GetByProductCodeAsync(product.ProductCode);
                if (existing != null)
                {
                    // Update existing product
                    existing.ProductCategory = product.ProductCategory;
                    existing.ProductType = product.ProductType;
                    existing.ColorTopSide = product.ColorTopSide;
                    existing.ColorBottomSide = product.ColorBottomSide;
                    existing.Grade = product.Grade;
                    existing.ZincCoating = product.ZincCoating;
                    existing.Thickness = product.Thickness;
                    existing.Width = product.Width;
                    existing.GrossWeight = product.GrossWeight;
                    existing.NetWeight = product.NetWeight;
                    existing.Defects = product.Defects;
                    existing.Price = product.Price;
                    _context.Update(existing);
                }
                else
                {
                    // Add new product
                    await _context.AddAsync(product);
                }
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
        public async Task<IEnumerable<Product>> GetAllAsync() 
            => await _context.Products
                            .Include(p => p.Bids)
                            .OrderByDescending(p => p.CreatedAt)
                            .ToListAsync();
        public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category)
        {
            return await _context.Products
                                .Where(p => p.ProductCategory == category && p.IsPublished)
                                .OrderByDescending(p => p.CreatedAt)
                                .ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
            => await _context.Products
                            .Include(p => p.Bids)
                            .FirstOrDefaultAsync(p => p.Id == id);
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
        public Task SaveChangesAsync() => _context.SaveChangesAsync();
        public async Task<IEnumerable<Product>> GetPublishedProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsPublished)
                .Include(p => p.Bids)
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Where(p => p.IsPublished && (
                    p.ProductCode.Contains(searchTerm) ||
                    p.Grade.Contains(searchTerm) ||
                    p.ColorTopSide.Contains(searchTerm) ||
                    p.ColorBottomSide.Contains(searchTerm) ||
                    p.Defects.Contains(searchTerm)))
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();
        }
        public async Task PublishProductAsync(int productId)
        {
            var product = await GetByIdAsync(productId);
            if (product != null)
            {
                product.IsPublished = true;
                product.PublishedAt = DateTime.UtcNow;
                await UpdateAsync(product);
            }
        }
        public async Task UnpublishProductAsync(int productId)
        {
            var product = await GetByIdAsync(productId);
            if (product != null)
            {
                product.IsPublished = false;
                await UpdateAsync(product);
            }
        }
    }
}
