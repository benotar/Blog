using System.Text.RegularExpressions;

namespace Blog.Application.Extensions;

public static partial class EnumExtensions
{
    public static string ToErrorString(this Enum enumValue)
    {
        var stringEnumValue = enumValue.ToString();

        return string.IsNullOrEmpty(stringEnumValue)
            ? stringEnumValue
            : MyRegex().Replace(stringEnumValue, " ");
    }

    [GeneratedRegex(@"(?<!^)(?=[A-Z])")]
    private static partial Regex MyRegex();
}