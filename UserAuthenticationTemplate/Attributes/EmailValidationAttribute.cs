using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace UserAuthenticationTemplate.Attributes
{
    public class EmailValidationAttribute : ValidationAttribute
    {
        private const string EmailRegex = @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))([0-9a-zA-Z][\w-]*\.)+[a-zA-Z]{2,63}$";

        public EmailValidationAttribute()
        {
            ErrorMessage = "Invalid email address format.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is string email && Regex.IsMatch(email, EmailRegex))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
