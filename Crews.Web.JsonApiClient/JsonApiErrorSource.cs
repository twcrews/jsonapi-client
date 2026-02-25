using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents the source of an error in a request, such as a specific field, parameter, or header that caused the
/// error.
/// </summary>
public record JsonApiErrorSource
{
    /// <summary>
    /// Gets or sets the JSON Pointer that identifies the location within a JSON document that caused the error.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("pointer")]
    public string? Pointer { get; init; }

    /// <summary>
    /// Gets or sets the URI query parameter value that caused the error.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("parameter")]
    public string? Parameter { get; init; }

    /// <summary>
    /// Gets or sets the name of a request header that caused the error.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("header")]
    public string? Header { get; init; }
}
