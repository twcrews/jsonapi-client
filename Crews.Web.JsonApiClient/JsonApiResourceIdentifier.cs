using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a reference to a resource by its type and identifier, as defined in section 7.3 of the JSON:API
/// specification.
/// </summary>
public class JsonApiResourceIdentifier
{
    /// <summary>
    /// Gets or sets the unique identifier for the object.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the local identifier associated with the object.
    /// </summary>
    [JsonPropertyName("lid")]
    public string? LocalId { get; set; }

    /// <summary>
    /// Gets or sets the type identifier for the object represented by this instance.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }
}
