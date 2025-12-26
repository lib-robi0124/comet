using Comet.DataAccess.Implementations;
using Comet.DataAccess.Interfaces;
using Comet.Domain.Entities;
using Comet.Domain.Enums;
using Comet.DTO.DTOs;
using Comet.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Comet.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IExcelReader _excelReader;
        public async Task ImportFromExcelAsync(List<ProductExcelRowDto> rows)
        {
            foreach (var row in rows)
            {
                var product = await _productRepository.GetByProductCodeAsync(row.ProductCode);

                if (product == null)
                {
                    product = new Product();
                    await _productRepository.AddAsync(product);
                }

                product.ProductCode = row.ProductCode;
                product.ProductCategory = Enum.Parse<ProductCategory>(row.ProductCategory);
                product.ProductType = Enum.Parse<ProductType>(row.ProductType);
                product.Price = row.Price;
            }

            await _productRepository.SaveChangesAsync();
        }
        private interface IExcelReader
        {
            IEnumerable<object> Read<T>(IFormFile file);
        }
    }
}
