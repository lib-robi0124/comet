using Comet.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.ModelsUser
{
    public class CreateBuyerUserVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string ContactPerson { get; set; }

        public Role Role { get; set; }

    }
}
