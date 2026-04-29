using System.Text;

namespace Blog.Application.Extensions;

public static class StringExtensions
{
    public static string ToLowerFistLetter(this string str)
        => string.IsNullOrEmpty(str) ? str : char.ToLower(str[0]) + str[1..];

    public static string ToUsername(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        var random = new Random();
        var randomNumber = random.Next(1000, 9999);

        var strBuilder = new StringBuilder();

        strBuilder.Append(str.Replace(' ', '_')).Append(randomNumber);

        return strBuilder.ToString().ToLower();
    }

    public static string ToUsernameBaseFromEmail(this string email)
    {
        if (email.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Email is required", nameof(email));
        }

        var atIndex = email.IndexOf('@');
        var localPart = atIndex > 0
            ? email[..atIndex]
            : email;

        var sanitized = new StringBuilder(localPart.Length);

        foreach (var c in localPart.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(c))
            {
                sanitized.Append(c);
            }
            else if (c is '.' or '+' or '-' or '_')
            {
                sanitized.Append('_');
            }
        }

        var result = sanitized.ToString().Trim('_');

        while (result.Contains("__"))
        {
            result = result.Replace("__", "_");
        }

        if (result.Length < 3)
        {
            result = $"user_{result}".TrimEnd('_');
        }

        if (result.Length > 24)
        {
            result = result[..24].TrimEnd('_');
        }

        return result;
    }

    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
}
