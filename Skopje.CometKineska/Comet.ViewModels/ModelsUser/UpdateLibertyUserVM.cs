using Comet.Domain.Entities;

namespace Comet.ViewModels.ModelsUser
{
    public class UpdateLibertyUserVM
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
