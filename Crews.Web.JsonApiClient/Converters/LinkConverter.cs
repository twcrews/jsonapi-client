using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient.Converters;

internal class LinkConverter : JsonConverter<Link>
{
    public override Link? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new Link { Href = reader.GetString()! };
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return JsonSerializer.Deserialize<Link>(ref reader, options);
        }

        throw new JsonException("Link must be a string or object");
    }

    public override void Write(Utf8JsonWriter writer, Link value, JsonSerializerOptions options)
    {
        // If only Href is set, serialize as string
        if (value.Rel == null && value.Metadata == null && value.Title == null &&
            value.Type == null && value.HrefLanguage == null && value.DescribedBy == null)
        {
            writer.WriteStringValue(value.Href);
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
