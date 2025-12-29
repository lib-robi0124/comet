using Comet.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.Models
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Display(Name = "Product Code")]
        public string ProductCode { get; set; } = string.Empty;

        [Display(Name = "Category")]
        public ProductCategory ProductCategory { get; set; }

        [Display(Name = "Type")]
        public ProductType ProductType { get; set; }

        [Display(Name = "Color Top Side")]
        public string ColorTopSide { get; set; } = string.Empty;

        [Display(Name = "Color Bottom Side")]
        public string ColorBottomSide { get; set; } = string.Empty;

        [Display(Name = "Grade")]
        public string Grade { get; set; } = string.Empty;

        [Display(Name = "Zinc Coating")]
        public string ZincCoating { get; set; } = string.Empty;

        [Display(Name = "Thickness (mm)")]
        public decimal Thickness { get; set; }

        [Display(Name = "Width (mm)")]
        public int Width { get; set; }

        [Display(Name = "Gross Weight (kg)")]
        public decimal GrossWeight { get; set; }

        [Display(Name = "Net Weight (kg)")]
        public decimal NetWeight { get; set; }

        [Display(Name = "Defects")]
        public string Defects { get; set; } = string.Empty;

        [Display(Name = "Price ($)")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Current Highest Bid (Eur)")]
        public decimal CurrentHighestBid { get; set; }
        [Display(Name = "Lyberty User")]
        public string FullName { get; set; } = string.Empty;
        [Display(Name = "Buyer User")]
        public string CompanyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
