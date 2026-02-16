using Crews.Web.JsonApiClient.Converters;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a hypermedia link with associated metadata, as defined in section 7.6.1 of the JSON:API specification.
/// </summary>
[JsonConverter(typeof(JsonApiLinkConverter))]
public class JsonApiLink
{
    /// <summary>
    /// Gets or sets the URL of the link.
    /// </summary>
    [JsonPropertyName("href")]
    public required Uri Href { get; set; }

    /// <summary>
    /// Gets or sets the relation type for the link.
    /// </summary>
    [JsonPropertyName("rel")]
    public string? Rel { get; set; }

    /// <summary>
    /// Gets or sets a link to a resource that provides additional descriptive information about the current object.
    /// </summary>
    [JsonPropertyName("describedby")]
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public JsonApiLink? DescribedBy { get; set; }

    /// <summary>
    /// Gets or sets the title associated with the object.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the type of the object represented by this instance.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the language of the linked resource, as defined by the hreflang attribute in HTML or XML sitemaps.
    /// </summary>
    [JsonPropertyName("hreflang")]
    public string? HrefLang { get; set; }

    /// <summary>
    /// Gets or sets metadata about the link.
    /// </summary>
    [JsonPropertyName("meta")]
    public JsonObject? Meta { get; set; }

    /// <summary>
    /// Implicitly converts a string URL to a JsonApiLink instance.
    /// </summary>
    /// <param name="href"></param>
    public static implicit operator JsonApiLink(string href) => new() { Href = new(href) };
}