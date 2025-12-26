using Comet.Domain.Entities;

namespace Comet.ViewModels.ModelsUser
{
    public class UserVM
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual bool CanSubmitPrice() => false;
    }
}
