using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.Auction
{
    public class BidVM
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Bid amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than 0")]
        [Display(Name = "Your Bid ($)")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Your Name / Company")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string CompanyName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public DateTime? BidTime { get; set; }

        // For tracking which user placed the bid
        public int UserId { get; set; }

        // Helper properties for validation
        public decimal? MinimumAllowedBid { get; set; }

        public string AmountDisplay => Amount.ToString("C");
    }
}
