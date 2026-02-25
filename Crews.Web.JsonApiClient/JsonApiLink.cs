using Crews.Web.JsonApiClient.Converters;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a hypermedia link with associated metadata, as defined in section 7.6.1 of the JSON:API specification.
/// </summary>
[JsonConverter(typeof(JsonApiLinkConverter))]
public record JsonApiLink
{
    /// <summary>
    /// Gets or sets the URL of the link.
    /// </summary>
    /// <remarks>
    /// This property is required by the JSON:API specification and must be present during deserialization.
    /// When constructing instances manually, ensure this property is initialized to avoid null reference exceptions.
    /// </remarks>
    [JsonRequired]
    [JsonPropertyName("href")]
    public Uri Href { get; init; } = null!;

    /// <summary>
    /// Gets or sets the relation type for the link.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rel")]
    public string? Rel { get; init; }

    /// <summary>
    /// Gets or sets a link to a resource that provides additional descriptive information about the current object.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("describedby")]
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public JsonApiLink? DescribedBy { get; init; }

    /// <summary>
    /// Gets or sets the title associated with the object.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Gets or sets the type of the object represented by this instance.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets or sets the language of the linked resource, as defined by the hreflang attribute in HTML or XML sitemaps.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("hreflang")]
    public string? HrefLang { get; init; }

    /// <summary>
    /// Gets or sets metadata about the link.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("meta")]
    public JsonObject? Meta { get; init; }

    /// <summary>
    /// Implicitly converts a string URL to a JsonApiLink instance.
    /// </summary>
    /// <param name="href">The URL string.</param>
    public static implicit operator JsonApiLink(string href) => new() { Href = new(href) };
}