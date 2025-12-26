using Comet.Domain.Entities;

namespace Comet.ViewModels.Models
{
    public class ProductDetailsVM
    {
        public ProductVM Product { get; set; } = null!;
        public List<BidVM> Bids { get; set; } = new();
        public decimal CurrentHighestBid { get; set; }
        public BidVM? UserBid { get; set; }
        public bool CanPlaceBid { get; set; }
    }
}
