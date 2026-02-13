namespace Comet.ViewModels.Auction
{  
        public class AuctionDashboardVM
        {
        public List<AuctionProductVM> Products { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string SelectedCategory { get; set; } = string.Empty;
        public BuyerStatsVM Stats { get; set; } = new();
        public List<CategoryFilterVM> Categories { get; set; } = new();
        public List<MyRecentBidVM> MyRecentBids { get; set; } = new();

    }
    
}
