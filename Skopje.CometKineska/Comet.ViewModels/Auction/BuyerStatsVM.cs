namespace Comet.ViewModels.Auction
{
    public class BuyerStatsVM
    {
        public int TotalActiveAuctions { get; set; }
        public int TotalMyBids { get; set; }
        public int WinningBids { get; set; }
        public decimal TotalBidValue { get; set; }
        public string TotalBidValueDisplay => TotalBidValue.ToString("C");
        public int OutbidCount => TotalMyBids - WinningBids;
        public double WinRate => TotalMyBids > 0 ? (double)WinningBids / TotalMyBids * 100 : 0;
    }
}
