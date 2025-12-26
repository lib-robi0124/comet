using Comet.Domain.Enums;
using LinqToExcel.Attributes;

namespace Comet.DTO.DTOs
{
    public class ProductExcelDto
    {
        [ExcelColumn("Product Code")]
        public string ProductCode { get; set; } = string.Empty;

        [ExcelColumn("Category")]
        public string CategoryText { get; set; } = string.Empty;

        [ExcelColumn("Product Type")]
        public string TypeText { get; set; } = string.Empty;

        [ExcelColumn("Color Top Side")]
        public string ColorTopSide { get; set; } = string.Empty;

        [ExcelColumn("Color Bottom Side")]
        public string ColorBottomSide { get; set; } = string.Empty;

        [ExcelColumn("Grade")]
        public string Grade { get; set; } = string.Empty;

        [ExcelColumn("Zinc Coating")]
        public string ZincCoating { get; set; } = string.Empty;

        [ExcelColumn("Thickness")]
        public string ThicknessText { get; set; } = string.Empty;

        [ExcelColumn("Width")]
        public string WidthText { get; set; } = string.Empty;

        [ExcelColumn("Gross Weight")]
        public string GrossWeightText { get; set; } = string.Empty;

        [ExcelColumn("Net Weight")]
        public string NetWeightText { get; set; } = string.Empty;

        [ExcelColumn("Defects")]
        public string Defects { get; set; } = string.Empty;

        [ExcelColumn("Price")]
        public string PriceText { get; set; } = string.Empty;

        // Helper methods for parsing
        public ProductCategory ParseCategory() =>
            Enum.Parse<ProductCategory>(CategoryText.Trim(), true);

        public ProductType ParseType() =>
            Enum.Parse<ProductType>(TypeText.Trim(), true);

        public decimal ParseThickness() =>
            decimal.TryParse(ThicknessText, out var value) ? value : 0;

        public int ParseWidth() =>
            int.TryParse(WidthText, out var value) ? value : 0;

        public decimal ParseGrossWeight() =>
            decimal.TryParse(GrossWeightText, out var value) ? value : 0;

        public decimal ParseNetWeight() =>
            decimal.TryParse(NetWeightText, out var value) ? value : 0;

        public decimal? ParsePrice() =>
            decimal.TryParse(PriceText, out var value) ? value : null;
    }
}
