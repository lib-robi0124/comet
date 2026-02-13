using Comet.ViewModels.Models;

namespace Comet.ViewModels.Auction
{
    public class ProductDetailsVM
    {
        public ProductVM Product { get; set; } = null!;
        public List<BidVM> Bids { get; set; } = new();
        public BidVM? UserBid { get; set; }
        public bool CanPlaceBid { get; set; }
        //new properties for enhanced details
        public int Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        // Pricing
        public decimal? StartingPrice { get; set; }
        public decimal? CurrentHighestBid { get; set; }

        // Product specifications
        public string ColorTopSide { get; set; } = string.Empty;
        public string ColorBottomSide { get; set; } = string.Empty;
        public string ZincCoating { get; set; } = string.Empty;
        public decimal Thickness { get; set; }
        public int Width { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string Defects { get; set; } = string.Empty;

        // Display properties
        public int Quantity => (int)GrossWeight;
        public string UnitOfMeasure => "kg";
        public string FullDescription => !string.IsNullOrEmpty(Defects) ? Defects :
            $"Thickness: {Thickness}mm, Width: {Width}mm, Colors: {ColorTopSide}/{ColorBottomSide}, Zinc: {ZincCoating}";
        public DateTime? AuctionEndDate { get; set; }
        public bool HasBids { get; set; }
        public int BidCount { get; set; }
        public List<BidHistoryVM> BidHistory { get; set; } = new();
        public bool IsAuctionActive => AuctionEndDate > DateTime.Now;
        public string TimeRemaining
        {
            get
            {
                if (!AuctionEndDate.HasValue) return "No end date";

                var timeLeft = AuctionEndDate.Value - DateTime.Now;
                if (timeLeft.TotalDays > 1)
                    return $"{timeLeft.Days} days left";
                else if (timeLeft.TotalHours > 1)
                    return $"{timeLeft.Hours} hours left";
                else if (timeLeft.TotalMinutes > 1)
                    return $"{timeLeft.Minutes} minutes left";
                else
                    return "Less than 1 minute";
            }
        }
        public decimal MinimumNextBid
        {
            get
            {
                if (CurrentHighestBid.HasValue && CurrentHighestBid.Value > 0)
                    return CurrentHighestBid.Value + 0.01m;
                if (StartingPrice.HasValue)
                    return StartingPrice.Value;
                return 0.01m;
            }
        }
        // For display in the view
        public string Specifications =>
            $"Thickness: {Thickness}mm | Width: {Width}mm | " +
            $"Colors: {ColorTopSide}/{ColorBottomSide} | " +
            $"Zinc: {ZincCoating} | " +
            $"Weight: {GrossWeight}kg (Gross) / {NetWeight}kg (Net)";

        public decimal? Price { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
