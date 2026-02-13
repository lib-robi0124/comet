namespace Comet.ViewModels.Models
{
    public class BidResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? BidId { get; set; }
        public decimal? NewCurrentBid { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
