using System.ComponentModel.DataAnnotations;
using UserAuthenticationTemplate.Attributes;

namespace UserAuthenticationTemplate.Models
{
    public class RegistrationRequest
    {
        [EmailValidation]
        public string? Email { get; set; }

        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }
}
