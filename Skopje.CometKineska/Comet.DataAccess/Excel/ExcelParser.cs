using Comet.Domain.Enums;
using Comet.DTO.DTOs;
using Comet.ViewModels.Models;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Reflection;

namespace Comet.DataAccess.Excel
{
    public class ExcelParser : IExcelParser
    {
        private readonly ILogger<ExcelParser> _logger;

        public ExcelParser(ILogger<ExcelParser> logger)
        {
            _logger = logger;
        }
        public List<T> Parse<T>(Stream excelStream) where T : class, new()
        {
            var result = new List<T>();
            using var package = new ExcelPackage(excelStream);
            var worksheet = package.Workbook.Worksheets[0];

            // Get properties with ExcelColumn attribute
            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<ExcelColumnAttribute>() != null)
                .ToDictionary(p => p.GetCustomAttribute<ExcelColumnAttribute>()!.ColumnName);

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var item = new T();
                bool hasData = false;

                foreach (var prop in properties)
                {
                    var cellValue = worksheet.Cells[row, GetColumnIndex(prop.Key, worksheet)]?.Text?.Trim();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        hasData = true;
                        var property = prop.Value;
                        var convertedValue = ConvertValue(cellValue, property.PropertyType);
                        property.SetValue(item, convertedValue);
                    }
                }
                if (hasData)
                    result.Add(item);
            }
            return result;
        }
        public async Task<ImportResult> ParseProductsAsync(Stream excelStream)
        {
            var result = new ImportResult
            {
                Success = true,
                ProcessingTime = TimeSpan.Zero
            };

            var startTime = DateTime.Now;

            try
            {
                using var package = new ExcelPackage(excelStream);
                var worksheet = package.Workbook.Worksheets[0];

                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    try
                    {
                        var dto = new ProductExcelDto
                        {
                            ProductCode = worksheet.Cells[row, 1]?.Text?.Trim() ?? string.Empty,
                            CategoryText = worksheet.Cells[row, 2]?.Text?.Trim() ?? string.Empty,
                            TypeText = worksheet.Cells[row, 3]?.Text?.Trim() ?? string.Empty,
                            ColorTopSide = worksheet.Cells[row, 4]?.Text?.Trim() ?? string.Empty,
                            ColorBottomSide = worksheet.Cells[row, 5]?.Text?.Trim() ?? string.Empty,
                            Grade = worksheet.Cells[row, 6]?.Text?.Trim() ?? string.Empty,
                            ZincCoating = worksheet.Cells[row, 7]?.Text?.Trim() ?? string.Empty,
                            ThicknessText = worksheet.Cells[row, 8]?.Text?.Trim() ?? string.Empty,
                            WidthText = worksheet.Cells[row, 9]?.Text?.Trim() ?? string.Empty,
                            GrossWeightText = worksheet.Cells[row, 10]?.Text?.Trim() ?? string.Empty,
                            NetWeightText = worksheet.Cells[row, 11]?.Text?.Trim() ?? string.Empty,
                            Defects = worksheet.Cells[row, 12]?.Text?.Trim() ?? string.Empty,
                            PriceText = worksheet.Cells[row, 13]?.Text?.Trim() ?? string.Empty
                        };

                        // Validate required fields
                        if (string.IsNullOrEmpty(dto.ProductCode))
                        {
                            throw new Exception("Product Code is required");
                        }

                        // Validate enum values
                        if (!Enum.TryParse<ProductCategory>(dto.CategoryText, true, out _))
                        {
                            throw new Exception($"Invalid category: {dto.CategoryText}");
                        }

                        if (!Enum.TryParse<ProductType>(dto.TypeText, true, out _))
                        {
                            throw new Exception($"Invalid product type: {dto.TypeText}");
                        }

                        result.SuccessfullyImported++;
                    }
                    catch (Exception ex)
                    {
                        result.FailedRows++;
                        result.Errors.Add(new ImportError
                        {
                            RowNumber = row,
                            ProductCode = worksheet.Cells[row, 1]?.Text?.Trim() ?? "N/A",
                            ErrorMessage = ex.Message
                        });
                        _logger.LogWarning(ex, "Error parsing row {Row}", row);
                    }
                }

                result.TotalRows = worksheet.Dimension.Rows - 1;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new ImportError
                {
                    RowNumber = 0,
                    ErrorMessage = $"Failed to parse Excel file: {ex.Message}"
                });
                _logger.LogError(ex, "Error parsing Excel file");
            }

            result.ProcessingTime = DateTime.Now - startTime;
            return result;
        }
        private int GetColumnIndex(string columnName, ExcelWorksheet worksheet)
        {
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                if (worksheet.Cells[1, col].Text?.Trim() == columnName)
                    return col;
            }
            throw new ArgumentException($"Column '{columnName}' not found in Excel sheet");
        }
        private object? ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            if (targetType == typeof(string))
                return value;

            if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            {
                if (decimal.TryParse(value, out var result))
                    return result;
                return targetType == typeof(decimal?) ? null : 0m;
            }

            if (targetType == typeof(int) || targetType == typeof(int?))
            {
                if (int.TryParse(value, out var result))
                    return result;
                return targetType == typeof(int?) ? null : 0;
            }

            if (targetType.IsEnum)
            {
                if (Enum.TryParse(targetType, value, true, out var result))
                    return result;
                return Enum.GetValues(targetType).GetValue(0);
            }

            return Convert.ChangeType(value, targetType);
        }
        [AttributeUsage(AttributeTargets.Property)]
        public class ExcelColumnAttribute : Attribute
        {
            public string ColumnName { get; }
            public ExcelColumnAttribute(string columnName)
            {
                ColumnName = columnName;
            }
        }
    }
}
