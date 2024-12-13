using System.ComponentModel.DataAnnotations;

namespace UserAuthenticationTemplate.Models
{
    public class LoginRequest
    {
        public string? Email { get; set; }

        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }
}
