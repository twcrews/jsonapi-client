using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Crews.Web.JsonApiClient.Tests")]
namespace Crews.Web.JsonApiClient.Converters;

internal class JsonApiLinkConverter : JsonConverter<JsonApiLink>
{
    public override JsonApiLink? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            string href = reader.GetString() ?? throw new JsonException("Href cannot be null.");
            return new() { Href = new(href) };
        }

        // Case 2: Link is an object with properties
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            JsonApiLink link = new() { Href = new(string.Empty, UriKind.Relative) };

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return link;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString() ?? throw new JsonException("Property name is null");
                    reader.Read();

                    switch (propertyName)
                    {
                        case "href":
                            string href = reader.GetString() ?? throw new JsonException("Href cannot be null.");
                            link.Href = new(href);
                            break;
                        case "rel":
                            link.Rel = reader.GetString();
                            break;
                        case "describedby":
                            link.DescribedBy = JsonSerializer.Deserialize<JsonApiLink>(ref reader, options);
                            break;
                        case "title":
                            link.Title = reader.GetString();
                            break;
                        case "type":
                            link.Type = reader.GetString();
                            break;
                        case "hreflang":
                            link.HrefLanguage = reader.GetString();
                            break;
                        case "meta":
                            link.Metadata = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
                            break;
                        default:
                            // Skip unknown properties
                            reader.Skip();
                            break;
                    }
                }
            }
        }

        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, JsonApiLink value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // If only Href is set, write as a simple string
        if (!string.IsNullOrEmpty(value.Href.OriginalString) &&
            string.IsNullOrEmpty(value.Rel) &&
            value.DescribedBy is null &&
            string.IsNullOrEmpty(value.Title) &&
            string.IsNullOrEmpty(value.Type) &&
            string.IsNullOrEmpty(value.HrefLanguage) &&
            value.Metadata is null)
        {
            writer.WriteStringValue(value.Href.OriginalString);
            return;
        }

        // Otherwise, write as an object
        writer.WriteStartObject();

        if (!string.IsNullOrEmpty(value.Href.OriginalString))
            writer.WriteString("href", value.Href.OriginalString);

        if (!string.IsNullOrEmpty(value.Rel))
            writer.WriteString("rel", value.Rel);

        if (value.DescribedBy is not null)
        {
            writer.WritePropertyName("describedby");
            JsonSerializer.Serialize(writer, value.DescribedBy, options);
        }

        if (!string.IsNullOrEmpty(value.Title))
            writer.WriteString("title", value.Title);

        if (!string.IsNullOrEmpty(value.Type))
            writer.WriteString("type", value.Type);

        if (!string.IsNullOrEmpty(value.HrefLanguage))
            writer.WriteString("hreflang", value.HrefLanguage);

        if (value.Metadata is not null)
        {
            writer.WritePropertyName("meta");
            JsonSerializer.Serialize(writer, value.Metadata, options);
        }

        writer.WriteEndObject();
    }
}
