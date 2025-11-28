using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a relationship object as defined in section 7.2.2.2 of the JSON:API specification.
/// </summary>
public class Relationship
{
    /// <summary>
    /// Gets or sets the <c>links</c> property of the relationship object.
    /// </summary>
    [JsonPropertyName("links")]
    public IEnumerable<Link>? Links { get; set; }

    /// <summary>
    /// Gets or sets the data payload associated with the response or request.
    /// </summary>
    [JsonPropertyName("data")]
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets additional metadata associated with the object.
    /// </summary>
    [JsonPropertyName("meta")]
    public JsonObject? Metadata { get; set; }

    /// <summary>
    /// Gets or sets members defined by any applied JSON:API extensions.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extensions { get; set; }
}
