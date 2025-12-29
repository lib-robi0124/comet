using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.ModelsUser
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        // Optional return URL after login
        public string? ReturnUrl { get; set; }
    }
}
