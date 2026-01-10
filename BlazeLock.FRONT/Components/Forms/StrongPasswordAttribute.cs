using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BlazeLock.FRONT.Components.Forms // Or your namespace
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;

            // Let [Required] handle empty strings
            if (string.IsNullOrEmpty(password))
                return ValidationResult.Success;

            int score = 0;
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(ch => !char.IsLetterOrDigit(ch))) score++;

            if (score < 2)
            {
                return new ValidationResult("Le mot de passe est trop faible. Ajoutez des majuscules, chiffres ou symboles.");
            }

            return ValidationResult.Success;
        }
    }
}