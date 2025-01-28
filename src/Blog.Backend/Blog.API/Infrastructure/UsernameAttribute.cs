using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Blog.API.Infrastructure;

public partial class UsernameAttribute() : ValidationAttribute(DefaultErrorMessage)
{
    private const string DefaultErrorMessage = "Username must be between 7 and 20 characters and cannot contain spaces";

    private static readonly Regex UsernameRegex = MyRegex();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || !UsernameRegex.IsMatch(value.ToString()))
        {
            return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
        }

        return ValidationResult.Success;
    }

    [GeneratedRegex(@"^[a-z0-9]{7,20}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}