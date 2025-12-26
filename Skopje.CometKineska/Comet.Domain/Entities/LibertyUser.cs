namespace Comet.Domain.Entities
{
    public class LibertyUser : User
    {
        public string FullName { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
