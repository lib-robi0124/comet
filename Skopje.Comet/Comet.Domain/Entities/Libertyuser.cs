using System.Data;

namespace Comet.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; } = true;
        public int? BuyerUserId { get; set; }
        public BuyerUser? BuyerUser { get; set; }
        public ICollection<NCOlist> NCOlists { get; set; }
        public User()
        {
            NCOlists = new HashSet<NCOlist>();
        }
    }
}
