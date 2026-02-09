using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.User
{
    public class UserRegisterDto
    {
        [Required] public string? Name { get; set; }
        [Required, EmailAddress] public string? Email { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare("Password")] public string ConfirmPassword { get; set; }
        [Required] public string Address { get; set; }
        [Required] public string Phone { get; set; }
    }
}
