namespace Comet.Domain.Entities
{
    public class Bid
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal OfferedPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Product Product { get; set; }
    }

}
