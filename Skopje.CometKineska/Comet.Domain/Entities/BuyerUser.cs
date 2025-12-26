namespace Comet.Domain.Entities
{
    public class BuyerUser : User
    {
        public string CompanyName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public ICollection<Bid> Bids { get; set; } = new HashSet<Bid>();
        public override bool CanSubmitPrice() => true;
    }
}
