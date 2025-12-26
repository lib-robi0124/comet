using Comet.Domain.Enums;

namespace Comet.DTO.DTOs
{
    public class ProductImportDto
    {
        public string ProductCode { get; set; }
        public ProductCategory Category { get; set; }
        public decimal Price { get; set; }
    }

}
