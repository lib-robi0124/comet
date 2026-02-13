namespace Comet.ViewModels.Auction
{
    public class MyRecentBidVM
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public decimal MyBidAmount { get; set; }
        public decimal CurrentHighestBid { get; set; }
        public DateTime BidTime { get; set; }
        public bool IsWinning { get; set; }

        public string Status => IsWinning ? "Winning" : "Outbid";
        public string StatusClass => IsWinning ? "success" : "warning";
        public string MyBidDisplay => MyBidAmount.ToString("C");
        public string CurrentBidDisplay => CurrentHighestBid.ToString("C");
        public string TimeDisplay => BidTime.ToString("MMM dd, HH:mm");
    }
}
