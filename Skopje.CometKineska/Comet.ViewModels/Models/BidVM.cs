using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.Models
{
    public class BidVM
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Bid amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than 0")]
        [Display(Name = "Your Bid ($)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Your Name")]
        [StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public DateTime? BidTime { get; set; }
    }
}
