namespace Blog.Application.Extensions;

public static class StringExtensions
{
    public static string ToLowerFistLetter(this string str)
        => string.IsNullOrEmpty(str) ? str : char.ToLower(str[0]) + str[1..];
}