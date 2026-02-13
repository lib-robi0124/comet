namespace Comet.ViewModels.Auction
{
    public class PlaceBidResultVM
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? BidId { get; set; }
        public decimal? NewCurrentBid { get; set; }
        public List<string> Errors { get; set; } = new();

        public string NewCurrentBidDisplay => NewCurrentBid?.ToString("C") ?? string.Empty;
        public bool HasErrors => Errors.Count > 0;
    }
}
