using System.Text.Json;
using System.Text.Json.Nodes;

namespace Crews.Web.JsonApiClient.Tests.Converters;

public class LinkConverterTests
{
    private readonly JsonSerializerOptions _options;

    public LinkConverterTests()
    {
        _options = new JsonSerializerOptions();
    }

    [Fact(DisplayName = "Read deserializes string value to Link with Href only")]
    public void ReadDeserializesStringValue()
    {
        const string json = "\"https://example.com/articles\"";

        Link? result = JsonSerializer.Deserialize<Link>(json, _options);

        Assert.NotNull(result);
        Assert.Equal("https://example.com/articles", result.Href);
        Assert.Null(result.Rel);
        Assert.Null(result.Title);
        Assert.Null(result.Type);
        Assert.Null(result.HrefLanguage);
        Assert.Null(result.Metadata);
        Assert.Null(result.DescribedBy);
    }

    [Fact(DisplayName = "Read deserializes object with href only")]
    public void ReadDeserializesObjectWithHrefOnly()
    {
        const string json = """{"href": "https://example.com/articles"}""";

        Link? result = JsonSerializer.Deserialize<Link>(json, _options);

        Assert.NotNull(result);
        Assert.Equal("https://example.com/articles", result.Href);
        Assert.Null(result.Rel);
        Assert.Null(result.Title);
        Assert.Null(result.Type);
        Assert.Null(result.HrefLanguage);
        Assert.Null(result.Metadata);
        Assert.Null(result.DescribedBy);
    }

    [Fact(DisplayName = "Read deserializes object with all properties")]
    public void ReadDeserializesObjectWithAllProperties()
    {
        const string json = """
        {
            "href": "https://example.com/articles",
            "rel": "self",
            "title": "Article Title",
            "type": "text/html",
            "hreflang": "en-US",
            "meta": {"count": 10}
        }
        """;

        Link? result = JsonSerializer.Deserialize<Link>(json, _options);

        Assert.NotNull(result);
        Assert.Equal("https://example.com/articles", result.Href);
        Assert.Equal("self", result.Rel);
        Assert.Equal("Article Title", result.Title);
        Assert.Equal("text/html", result.Type);
        Assert.Equal("en-US", result.HrefLanguage);
        Assert.NotNull(result.Metadata);
        Assert.Equal(10, result.Metadata["count"]!.GetValue<int>());
        Assert.Null(result.DescribedBy);
    }

    [Fact(DisplayName = "Read deserializes object with nested describedby link")]
    public void ReadDeserializesObjectWithNestedDescribedBy()
    {
        const string json = """
      {
                "href": "https://example.com/articles",
     "describedby": {
              "href": "https://example.com/schema/article",
     "title": "Article Schema"
     }
     }
     """;

        Link? result = JsonSerializer.Deserialize<Link>(json, _options);

        Assert.NotNull(result);
        Assert.Equal("https://example.com/articles", result.Href);
        Assert.NotNull(result.DescribedBy);
        Assert.Equal("https://example.com/schema/article", result.DescribedBy.Href);
        Assert.Equal("Article Schema", result.DescribedBy.Title);
    }

    [Fact(DisplayName = "Read throws JsonException for invalid token type")]
    public void ReadThrowsExceptionForInvalidTokenType()
    {
        const string json = "123";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Link>(json, _options));
    }

    [Fact(DisplayName = "Read throws JsonException for array token type")]
    public void ReadThrowsExceptionForArrayTokenType()
    {
        const string json = "[]";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Link>(json, _options));
    }

    [Fact(DisplayName = "Write serializes Link with href only as string")]
    public void WriteSerializesHrefOnlyAsString()
    {
        Link link = new() { Href = "https://example.com/articles" };

        string result = JsonSerializer.Serialize(link, _options);

        Assert.Equal("\"https://example.com/articles\"", result);
    }

    [Fact(DisplayName = "Write serializes Link with rel as object")]
    public void WriteSerializesLinkWithRelAsObject()
    {
        Link link = new()
        {
            Href = "https://example.com/articles",
            Rel = "self"
        };

        string result = JsonSerializer.Serialize(link, _options);

        JsonDocument doc = JsonDocument.Parse(result);
        Assert.Equal("https://example.com/articles", doc.RootElement.GetProperty("href").GetString());
        Assert.Equal("self", doc.RootElement.GetProperty("rel").GetString());
    }

    [Fact(DisplayName = "Write serializes Link with title as object")]
    public void WriteSerializesLinkWithTitleAsObject()
    {
        Link link = new()
        {
            Href = "https://example.com/articles",
            Title = "Article Title"
        };

        string result = JsonSerializer.Serialize(link, _options);

        JsonDocument doc = JsonDocument.Parse(result);
        Assert.Equal("https://example.com/articles", doc.RootElement.GetProperty("href").GetString());
        Assert.Equal("Article Title", doc.RootElement.GetProperty("title").GetString());
    }

    [Fact(DisplayName = "Write serializes Link with type as object")]
    public void WriteSerializesLinkWithTypeAsObject()
    {
        Link link = new()
        {
            Href = "https://example.com/articles",
            Type = "text/html"
        };

        string result = JsonSerializer.Serialize(link, _options);

        JsonDocument doc = JsonDocument.Parse(result);
        Assert.Equal("https://example.com/articles", doc.RootElement.GetProperty("href").GetString());
        Assert.Equal("text/html", doc.RootElement.GetProperty("type").GetString());
    }

    [Fact(DisplayName = "Write serializes Link with hreflang as object")]
    public void WriteSerializesLinkWithHrefLangAsObject()
    {
        Link link = new()
        {
            Href = "https://example.com/articles",
            HrefLanguage = "en-US"
        };

        string result = JsonSerializer.Serialize(link, _options);

        JsonDocument doc = JsonDocument.Parse(result);
        Assert.Equal("https://example.com/articles", doc.RootElement.GetProperty("href").GetString());
        Assert.Equal("en-US", doc.RootElement.GetProperty("hreflang").GetString());
    }

    [Fact(DisplayName = "Write serializes Link with metadata as object")]
    public void WriteSerializesLinkWithMetadataAsObject()
    {
        JsonObject metadata = new() { ["count"] = 10 };
        Link link = new()
        {
            Href = "https://example.com/articles",
            Metadata = metadata
        };

        string result = JsonSerializer.Serialize(link, _options);

        JsonDocument doc = JsonDocument.Parse(result);
        Assert.Equal("https://example.com/articles", doc.RootElement.GetProperty("href").GetString());
        Assert.Equal(10, doc.RootElement.GetProperty("meta").GetProperty("count").GetInt32());
    }

    [Fact(DisplayName = "Write serializes Link with describedby as object")]
    public void WriteSerializesLinkWithDescribedByAsObject()
    {
        Link describedBy = new()
        {
            Href = "https://example.com/schema/article",
            Title = "Article Schema"
        };
        Link link = new()
        {
            Href = "https://example.com/articles",
            DescribedBy = describedBy
        };

        string result = JsonSerializer.Serialize(link, _options);

        JsonDocument doc = JsonDocument.Parse(result);
        Assert.Equal("https://example.com/articles", doc.RootElement.GetProperty("href").GetString());
        JsonElement describedByElement = doc.RootElement.GetProperty("describedby");
        Assert.Equal("https://example.com/schema/article", describedByElement.GetProperty("href").GetString());
        Assert.Equal("Article Schema", describedByElement.GetProperty("title").GetString());
    }

    [Fact(DisplayName = "Write serializes Link with all properties as object")]
    public void WriteSerializesLinkWithAllPropertiesAsObject()
    {
        JsonObject metadata = new() { ["count"] = 10 };
        Link describedBy = new()
        {
            Href = "https://example.com/schema/article"
        };
        Link link = new()
        {
            Href = "https://example.com/articles",
            Rel = "self",
            Title = "Article Title",
            Type = "text/html",
            HrefLanguage = "en-US",
            Metadata = metadata,
            DescribedBy = describedBy
        };

        string result = JsonSerializer.Serialize(link, _options);

        JsonDocument doc = JsonDocument.Parse(result);
        Assert.Equal("https://example.com/articles", doc.RootElement.GetProperty("href").GetString());
        Assert.Equal("self", doc.RootElement.GetProperty("rel").GetString());
        Assert.Equal("Article Title", doc.RootElement.GetProperty("title").GetString());
        Assert.Equal("text/html", doc.RootElement.GetProperty("type").GetString());
        Assert.Equal("en-US", doc.RootElement.GetProperty("hreflang").GetString());
        Assert.Equal(10, doc.RootElement.GetProperty("meta").GetProperty("count").GetInt32());
        Assert.Equal("https://example.com/schema/article", doc.RootElement.GetProperty("describedby").GetProperty("href").GetString());
    }

    [Theory(DisplayName = "Roundtrip serialization preserves Link data")]
    [InlineData("https://example.com/simple")]
    [InlineData("https://example.com/with-path/and/params?id=123&type=article")]
    public void RoundtripSerializationPreservesSimpleLink(string href)
    {
        Link original = new() { Href = href };

        string json = JsonSerializer.Serialize(original, _options);
        Link? deserialized = JsonSerializer.Deserialize<Link>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(original.Href, deserialized.Href);
        Assert.Equal(original.Rel, deserialized.Rel);
        Assert.Equal(original.Title, deserialized.Title);
        Assert.Equal(original.Type, deserialized.Type);
        Assert.Equal(original.HrefLanguage, deserialized.HrefLanguage);
        Assert.Equal(original.Metadata, deserialized.Metadata);
        Assert.Equal(original.DescribedBy, deserialized.DescribedBy);
    }

    [Fact(DisplayName = "Roundtrip serialization preserves complex Link data")]
    public void RoundtripSerializationPreservesComplexLink()
    {
        JsonObject metadata = new() { ["count"] = 42, ["type"] = "article" };
        Link original = new()
        {
            Href = "https://example.com/articles",
            Rel = "self",
            Title = "Article Collection",
            Type = "application/json",
            HrefLanguage = "en-US",
            Metadata = metadata
        };

        string json = JsonSerializer.Serialize(original, _options);
        Link? deserialized = JsonSerializer.Deserialize<Link>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(original.Href, deserialized.Href);
        Assert.Equal(original.Rel, deserialized.Rel);
        Assert.Equal(original.Title, deserialized.Title);
        Assert.Equal(original.Type, deserialized.Type);
        Assert.Equal(original.HrefLanguage, deserialized.HrefLanguage);
        Assert.NotNull(deserialized.Metadata);
        Assert.Equal(42, deserialized.Metadata["count"]!.GetValue<int>());
        Assert.Equal("article", deserialized.Metadata["type"]!.GetValue<string>());
    }
}
