using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Required(ErrorMessage = "Password confirmation is required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
