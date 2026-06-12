using System.Text.Json;
using System.Text.Json.Serialization;

namespace Review_Guard.API.Extensions;

public sealed class DecimalJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.GetDecimal();
    }

    public override void Write(
        Utf8JsonWriter writer,
        decimal value,
        JsonSerializerOptions options)
    {
        writer.WriteNumberValue(decimal.Parse(
            value.ToString("0.##")));
    }
}