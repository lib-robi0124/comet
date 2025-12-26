using Comet.DataAccess.Excel;
using Comet.DataAccess.Interfaces;
using Comet.Domain.Entities;
using Comet.Domain.Enums;
using Comet.DTO.DTOs;
using Comet.Services.Interfaces;

namespace Comet.Services.Implementations
{
    public class ProductImportService : IProductImportService
    {
        private readonly IExcelParser _parser;
        private readonly IProductRepository _repo;

        public ProductImportService(IExcelParser parser, IProductRepository repo)
        {
            _parser = parser;
            _repo = repo;
        }

        public async Task<ImportResultDto> ImportFromExcelAsync(Stream stream, bool overwrite)
        {
            var rows = _parser.Parse<ProductExcelImportDto>(stream);
            var result = new ImportResultDto { TotalRows = rows.Count };

            var products = new List<Product>();

            foreach (var row in rows)
            {
                try
                {
                    var product = new Product
                    {
                        ProductCode = row.ProductCode,
                        ProductCategory = Enum.Parse<ProductCategory>(row.ProductCategory),
                        ProductType = Enum.Parse<ProductType>(row.ProductType),
                        ColorTopSide = row.ColorTopSide,
                        ColorBottomSide = row.ColorBottomSide,
                        Grade = row.Grade,
                        ZincCoating = row.ZincCoating,
                        Thickness = decimal.Parse(row.Thickness),
                        Width = int.Parse(row.Width),
                        GrossWeight = decimal.Parse(row.GrossWeight),
                        NetWeight = decimal.Parse(row.NetWeight),
                        Defects = row.Defects,
                        Price = decimal.Parse(row.Price)
                    };

                    products.Add(product);
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Product {row.ProductCode}: {ex.Message}");
                }
            }

            await _repo.BulkInsertOrUpdateAsync(products);
            await _repo.SaveChangesAsync();

            result.Inserted = products.Count;
            return result;
        }
    }
}
