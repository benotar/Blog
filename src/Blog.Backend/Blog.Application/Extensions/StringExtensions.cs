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
}