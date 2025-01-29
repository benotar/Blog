using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Blog.API.Infrastructure;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public partial class UsernameAttribute() : ValidationAttribute(DefaultErrorMessage)
{
    private const string DefaultErrorMessage = "Username must be between 7 and 20 characters and cannot contain spaces";

    private static readonly Regex UsernameRegex = MyRegex();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        
        return !UsernameRegex.IsMatch(value.ToString())
            ? new ValidationResult(ErrorMessage ?? DefaultErrorMessage)
            : ValidationResult.Success;
    }

    [GeneratedRegex(@"^[a-z0-9_]{7,20}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}