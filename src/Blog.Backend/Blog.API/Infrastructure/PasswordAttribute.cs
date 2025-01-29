using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Blog.API.Infrastructure;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public partial class PasswordAttribute() : ValidationAttribute(DefaultErrorMessage)
{
    private const string DefaultErrorMessage =
        "Password must contain at least one uppercase letter, one lowercase letter, one number and a minimum of 8 characters.";

    private static readonly Regex PasswordRegex = MyRegex();
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        
        return !PasswordRegex.IsMatch(value.ToString())
            ? new ValidationResult(ErrorMessage ?? DefaultErrorMessage)
            : ValidationResult.Success;
    }

    [GeneratedRegex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{8,}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}