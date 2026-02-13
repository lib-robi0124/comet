namespace Comet.ViewModels.Auction
{
    public class BidHistoryVM
    {
        public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
        public bool IsCurrentUser { get; set; }

        public string AmountDisplay => Amount.ToString("C");
        public string TimeDisplay => BidTime.ToString("MMM dd, yyyy HH:mm:ss");
        public string RowClass => IsCurrentUser ? "table-primary" : "";
    }
}
