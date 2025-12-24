using Comet.Domain.Enums;

namespace Comet.Domain.Entities
{
    public class BuyerUser
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Country Country { get; set; }
        public ICollection<NCOList> NCOlists { get; set; } = new HashSet<NCOList>();
    }
}
