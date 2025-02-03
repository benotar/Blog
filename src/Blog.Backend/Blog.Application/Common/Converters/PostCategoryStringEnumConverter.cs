using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blog.Application.Common.Converters;

public class PostCategoryStringEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        return Enum.TryParse<TEnum>(value, true, out TEnum result) ? result : default;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}