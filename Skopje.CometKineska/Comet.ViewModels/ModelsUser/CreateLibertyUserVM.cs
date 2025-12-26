using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.ModelsUser
{
    public class CreateLibertyUserVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        public string Position { get; set; }
        public List<string> Permissions { get; set; }
    }
}
