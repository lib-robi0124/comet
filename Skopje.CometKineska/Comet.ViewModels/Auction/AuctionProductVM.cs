namespace Comet.ViewModels.Auction
{
    public class AuctionProductVM
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;

        // From ProductVM
        public decimal? StartingPrice { get; set; } // Maps to Price in ProductVM
        public decimal? CurrentHighestBid { get; set; } // Maps to CurrentHighestBid in ProductVM

        // Product specifications from ProductVM
        public string ColorTopSide { get; set; } = string.Empty;
        public string ColorBottomSide { get; set; } = string.Empty;
        public string ZincCoating { get; set; } = string.Empty;
        public decimal Thickness { get; set; }
        public int Width { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string Defects { get; set; } = string.Empty;

        // For display purposes
        public int Quantity => (int)GrossWeight; // Using GrossWeight as quantity
        public string UnitOfMeasure => "kg";

        // Combined description
        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(Defects))
                    return Defects;

                return $"Thickness: {Thickness}mm, Width: {Width}mm, " +
                       $"Colors: {ColorTopSide}/{ColorBottomSide}, Zinc: {ZincCoating}";
            }
        }
        public DateTime? AuctionEndDate { get; set; }
        public bool HasBids { get; set; }
        public int BidCount { get; set; }
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
                else if (timeLeft.TotalSeconds > 0)
                    return "Less than 1 minute";
                else
                    return "Auction ended";
            }
        }
        public string CurrentBidDisplay => CurrentHighestBid?.ToString("C") ?? "No bids";
        public string StartingPriceDisplay => StartingPrice?.ToString("C") ?? "Not set";

        // Additional display properties for product specs
        public string Specifications =>
            $"{Thickness}mm x {Width}mm | {ColorTopSide}/{ColorBottomSide} | Zinc: {ZincCoating}";
    }
}

