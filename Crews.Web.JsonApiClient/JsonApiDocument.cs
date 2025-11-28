using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a base class for JSON:API top-level objects as defined in section 7.1 of the JSON:API specification.
/// </summary>
public abstract class JsonApiDocument
{
    /// <summary>
    /// Gets or sets the <c>jsonapi</c> property of the document.
    /// </summary>
    [JsonPropertyName("jsonapi")]
    public JsonApiInfo? JsonApi { get; set; }

    /// <summary>
    /// Gets or sets the primary data payload associated with the document.
    /// </summary>
    public JsonObject? Data { get; set; }

    /// <summary>
    /// Gets or sets the collection of errors associated with the document.
    /// </summary>
    [JsonPropertyName("errors")]
    public IEnumerable<Error>? Errors { get; set; }

    /// <summary>
    /// Gets or sets the <c>links</c> property of the document.
    /// </summary>
    /// <seealso href="https://jsonapi.org/format/#document-links"/>
    [JsonPropertyName("links")]
    public LinksObject? Links { get; set; }

    /// <summary>
    /// Gets or sets the <c>meta</c> property of the document.
    /// </summary>
    /// <seealso href="https://jsonapi.org/format/#document-meta"/>
    [JsonPropertyName("meta")]
    public JsonObject? Metadata { get; set; }

    /// <summary>
    /// Gets or sets members defined by any applied JSON:API extensions.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extensions { get; set; }
}
