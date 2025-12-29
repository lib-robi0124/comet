namespace Comet.Domain.Entities
{
    public class Bid
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Amount { get; set; }
        public int? BuyerUserId { get; set; }
        public BuyerUser? BuyerUser { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime BidTime { get; set; } = DateTime.UtcNow;
        public bool IsWinningBid { get; set; } = false;
        public Product Product { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
    }
}
