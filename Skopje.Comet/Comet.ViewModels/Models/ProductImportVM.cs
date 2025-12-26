using Microsoft.AspNetCore.Http;

namespace Comet.ViewModels.Models
{
    public class ProductImportVM
    {
        // Map to Excel columns
        public string ProductCode { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty; // Will parse to enum
        public string ProductType { get; set; } = string.Empty;    // Will parse to enum
        public string ColorTopSide { get; set; } = string.Empty;
        public string ColorBottomSide { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string ZincCoating { get; set; } = string.Empty;
        public string Thickness { get; set; } = string.Empty;  // String for parsing
        public string Width { get; set; } = string.Empty;      // String for parsing
        public string GrossWeight { get; set; } = string.Empty; // Fixed spelling
        public string NetWeight { get; set; } = string.Empty;   // Fixed spelling
        public string Defects { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;      // String for parsing
        public IFormFile ExcelFile { get; set; }
        public List<ProductRowVM> Rows { get; set; } // optional preview
    }

}
