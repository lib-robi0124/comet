using Comet.DataAccess.Excel;
using Comet.DataAccess.Interfaces;
using Comet.Domain.Entities;
using Comet.Domain.Enums;
using Comet.DTO.DTOs;
using Comet.Services.Interfaces;
using Comet.ViewModels.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Comet.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IExcelParser _excelParser;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            IExcelParser excelParser,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _excelParser = excelParser;
            _logger = logger;
        }
        public async Task<ImportResult> ImportFromExcelAsync(Stream excelStream, bool overwriteExisting)
        {
            var importResult = await _excelParser.ParseProductsAsync(excelStream);

            if (!importResult.Success || importResult.SuccessfullyImported == 0)
                return importResult;

            try
            {
                // Reset stream position and parse to DTOs
                excelStream.Position = 0;
                var excelDtos = _excelParser.Parse<ProductExcelDto>(excelStream);

                var products = excelDtos.Select(dto => new Product
                {
                    ProductCode = dto.ProductCode,
                    ProductCategory = dto.ParseCategory(),
                    ProductType = dto.ParseType(),
                    ColorTopSide = dto.ColorTopSide,
                    ColorBottomSide = dto.ColorBottomSide,
                    Grade = dto.Grade,
                    ZincCoating = dto.ZincCoating,
                    Thickness = dto.ParseThickness(),
                    Width = dto.ParseWidth(),
                    GrossWeight = dto.ParseGrossWeight(),
                    NetWeight = dto.ParseNetWeight(),
                    Defects = dto.Defects,
                    Price = dto.ParsePrice(),
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _productRepository.BulkInsertOrUpdateAsync(products);

                importResult.Success = true;
                _logger.LogInformation("Successfully imported {Count} products", products.Count);
            }
            catch (Exception ex)
            {
                importResult.Success = false;
                importResult.Errors.Add(new ImportError
                {
                    ErrorMessage = $"Failed to save products: {ex.Message}"
                });
                _logger.LogError(ex, "Error saving imported products");
            }

            return importResult;
        }
        public async Task<IEnumerable<ProductVM>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => MapToViewModel(p));
        }
        public async Task<ProductVM?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToViewModel(product) : null;
        }
        private ProductVM MapToViewModel(Product product)
        {
            return new ProductVM
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                ProductCategory = product.ProductCategory,
                ProductType = product.ProductType,
                ColorTopSide = product.ColorTopSide,
                ColorBottomSide = product.ColorBottomSide,
                Grade = product.Grade,
                ZincCoating = product.ZincCoating,
                Thickness = product.Thickness,
                Width = product.Width,
                GrossWeight = product.GrossWeight,
                NetWeight = product.NetWeight,
                Defects = product.Defects,
                Price = product.Price,
                IsPublished = product.IsPublished,
                CreatedAt = product.CreatedAt,
                PublishedAt = product.PublishedAt
            };
        }
        public async Task<bool> UpdateProductAsync(ProductVM viewModel)
        {
            var product = await _productRepository.GetByIdAsync(viewModel.Id);
            if (product == null)
                return false;

            // Update product properties
            product.ProductCode = viewModel.ProductCode;
            product.ProductCategory = viewModel.ProductCategory;
            product.ProductType = viewModel.ProductType;
            product.ColorTopSide = viewModel.ColorTopSide;
            product.ColorBottomSide = viewModel.ColorBottomSide;
            product.Grade = viewModel.Grade;
            product.ZincCoating = viewModel.ZincCoating;
            product.Thickness = viewModel.Thickness;
            product.Width = viewModel.Width;
            product.GrossWeight = viewModel.GrossWeight;
            product.NetWeight = viewModel.NetWeight;
            product.Defects = viewModel.Defects;
            product.Price = viewModel.Price;
            product.IsPublished = viewModel.IsPublished;

            await _productRepository.UpdateAsync(product);
            return true;
        }
        public async Task<bool> PublishProductAsync(int id)
        {
            try
            {
                await _productRepository.PublishProductAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing product {ProductId}", id);
                return false;
            }
        }
        public async Task<bool> UnpublishProductAsync(int id)
        {
            try
            {
                await _productRepository.UnpublishProductAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing product {ProductId}", id);
                return false;
            }
        }
        public async Task<IEnumerable<ProductVM>> GetPublishedProductsAsync()
        {
            var products = await _productRepository.GetPublishedProductsAsync();
            return products.Select(p => MapToViewModel(p));
        }
        public async Task<ProductDetailsVM> GetProductDetailsAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            var viewModel = new ProductDetailsVM
            {
                Product = MapToViewModel(product),
                CurrentHighestBid = product.Bids.Any() ? product.Bids.Max(b => b.Amount) : 0,
                CanPlaceBid = product.IsPublished && product.Price.HasValue,
                Bids = product.Bids
                    .OrderByDescending(b => b.Amount)
                    .ThenBy(b => b.BidTime)
                    .Select(b => new BidVM
                    {
                        Amount = b.Amount,
                        BidderName = b.BidderName,
                        BidderEmail = b.BidderEmail
                    }).ToList()
            };
            return viewModel;
        }
        public async Task<Stream> GenerateTemplateAsync()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Product Template");

            // Headers
            worksheet.Cells[1, 1].Value = "Product Code";
            worksheet.Cells[1, 2].Value = "Category";
            worksheet.Cells[1, 3].Value = "Product Type";
            worksheet.Cells[1, 4].Value = "Color Top Side";
            worksheet.Cells[1, 5].Value = "Color Bottom Side";
            worksheet.Cells[1, 6].Value = "Grade";
            worksheet.Cells[1, 7].Value = "Zinc Coating";
            worksheet.Cells[1, 8].Value = "Thickness";
            worksheet.Cells[1, 9].Value = "Width";
            worksheet.Cells[1, 10].Value = "Gross Weight";
            worksheet.Cells[1, 11].Value = "Net Weight";
            worksheet.Cells[1, 12].Value = "Defects";
            worksheet.Cells[1, 13].Value = "Price";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 13])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Add validation for enums
            var categoryValidation = worksheet.DataValidations.AddListValidation("B2:B1000");
            foreach (var category in Enum.GetNames<ProductCategory>())
            {
                categoryValidation.Formula.Values.Add(category);
            }

            var typeValidation = worksheet.DataValidations.AddListValidation("C2:C1000");
            foreach (var type in Enum.GetNames<ProductType>())
            {
                typeValidation.Formula.Values.Add(type);
            }

            //// Add sample data
            //worksheet.Cells[2, 1].Value = "PROD-001";
            //worksheet.Cells[2, 2].Value = "Steel";
            //worksheet.Cells[2, 3].Value = "Sheet";
            //worksheet.Cells[2, 4].Value = "Red";
            //worksheet.Cells[2, 5].Value = "White";
            //worksheet.Cells[2, 6].Value = "A36";
            //worksheet.Cells[2, 7].Value = "Z275";
            //worksheet.Cells[2, 8].Value = 1.5;
            //worksheet.Cells[2, 9].Value = 1000;
            //worksheet.Cells[2, 10].Value = 500;
            //worksheet.Cells[2, 11].Value = 480;
            //worksheet.Cells[2, 12].Value = "Minor scratches";
            //worksheet.Cells[2, 13].Value = 1000;

            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0;

            return stream;
        }
    }
}
