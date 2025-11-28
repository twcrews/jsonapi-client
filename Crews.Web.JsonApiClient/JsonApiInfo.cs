using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents information about the JSON:API implementation as defined in section 7.7 of the JSON:API specification.
/// </summary>
public class JsonApiInfo
{
    /// <summary>
    /// Gets or sets the JSON:API version for the document.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the collection of extension URIs associated with the document.
    /// </summary>
    [JsonPropertyName("ext")]
    public IEnumerable<Uri>? Extensions { get; set; }

    /// <summary>
    /// Gets or sets the collection of profile URIs associated with the document.
    /// </summary>
    [JsonPropertyName("profile")]
    public IEnumerable<Uri>? Profiles { get; set; }

    /// <summary>
    /// Gets or sets the collection of additional metadata associated with the object.
    /// </summary>
    [JsonPropertyName("meta")]
    public JsonObject? Metadata { get; set; }
}
