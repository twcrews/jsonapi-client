using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents the source of an error in a request, such as a specific field, parameter, or header that caused the
/// error.
/// </summary>
public class JsonApiErrorSource
{
    /// <summary>
    /// Gets or sets the JSON Pointer that identifies the location within a JSON document that caused the error.
    /// </summary>
    [JsonPropertyName("pointer")]
    public string? Pointer { get; set; }

    /// <summary>
    /// Gets or sets the URI query parameter value that caused the error.
    /// </summary>
    [JsonPropertyName("parameter")]
    public string? Parameter { get; set; }

    /// <summary>
    /// Gets or sets the name of a request header that caused the error.
    /// </summary>
    [JsonPropertyName("header")]
    public string? Header { get; set; }
}
