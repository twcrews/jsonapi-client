using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a resource object as defined in section 7.2 of the JSON:API specification.
/// </summary>
public class Resource : ResourceIdentifier
{
    /// <summary>
    /// Gets or sets the collection of custom attributes associated with this object.
    /// </summary>
    [JsonPropertyName("attributes")]
    public JsonObject? Attributes { get; set; }

    /// <summary>
    /// Gets or sets the collection of relationships associated with this object.
    /// </summary>
    [JsonPropertyName("relationships")]
    public Dictionary<string, Relationship>? Relationships { get; set; }

    /// <summary>
    /// Gets or sets the <c>links</c> property of the resource object.
    /// </summary>
    [JsonPropertyName("links")]
    public LinksObject? Links { get; set; }

    /// <summary>
    /// Gets or sets the custom metadata associated with this object.
    /// </summary>
    [JsonPropertyName("meta")]
    public JsonObject? Metadata { get; set; }
}
