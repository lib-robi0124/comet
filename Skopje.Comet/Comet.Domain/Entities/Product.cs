using Comet.Domain.Enums;

namespace Comet.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public ProductCategory ProductCategory { get; set; }
        public ProductType ProductType { get; set; }
        public string ColorTopSide { get; set; } = string.Empty;
        public string ColorBottomSide { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string ZincCoating { get; set; } = string.Empty;
        public decimal Thickness { get; set; }
        public int Width { get; set; }
        public decimal GrossWeight { get; set; } 
        public decimal NetWeight { get; set; }
        public string Defects { get; set; } = string.Empty;
        public decimal? Price { get; set; }
    }
}
