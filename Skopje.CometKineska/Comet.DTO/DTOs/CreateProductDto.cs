using Comet.Domain.Enums;

namespace Comet.DTO.DTOs
{
    public class CreateProductDto
    {
        public string ProductCode { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public ProductType ProductType { get; set; }
        public string ColorTopSide { get; set; }
        public string ColorBottomSide { get; set; }
        public string Grade { get; set; }
        public string ZincCoating { get; set; }
        public int Thickness { get; set; }
        public int Width { get; set; }
        public int GrossWeight { get; set; }
        public int NetWeight { get; set; }
        public string Defects { get; set; }
        public decimal Price { get; set; }
    }
}
