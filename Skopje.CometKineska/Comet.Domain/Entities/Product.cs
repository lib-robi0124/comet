using Comet.Domain.Enums;
using System.Security.Cryptography;

namespace Comet.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int? LibertyUserId { get; set; }
        public LibertyUser? LibertyUser { get; set; }
        public int? BuyerUserId { get; set; }
        public BuyerUser? BuyerUser { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public ProductCategory ProductCategory { get; set; }
        public ProductType ProductType { get; set; }
        public string ColorTopSide { get; set; } = string.Empty;
        public string? ColorBottomSide { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string ZincCoating { get; set; } = string.Empty;
        public decimal Thickness { get; set; }
        public int Width { get; set; }
        public decimal GrossWeight { get; set; } 
        public decimal NetWeight { get; set; }
        public string Defects { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAt { get; set; }
        public DateTime AuctionStartTime { get; set; }
        public decimal MinimumBidPrice { get; set; }
    }
}
