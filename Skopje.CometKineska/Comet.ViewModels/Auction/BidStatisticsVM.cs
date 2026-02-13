namespace Comet.ViewModels.Auction
{
    public class BidStatisticsVM
    {
        public int TotalBids { get; set; }
        public decimal? StartingPrice { get; set; }
        public decimal? CurrentHighestBid { get; set; }
        public decimal? AverageBid { get; set; }
        public int UniqueBidders { get; set; }
        public DateTime? LastBidTime { get; set; }

        public string StartingPriceDisplay => StartingPrice?.ToString("C") ?? "Not set";
        public string CurrentHighestBidDisplay => CurrentHighestBid?.ToString("C") ?? "No bids";
        public string AverageBidDisplay => AverageBid?.ToString("C") ?? "N/A";
        public string LastBidTimeDisplay => LastBidTime?.ToString("MMM dd, HH:mm") ?? "Never";
    }
}
