using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents an error in a document as defined in section 11.2 of the JSON:API specification.
/// </summary>
public class JsonApiError
{
    /// <summary>
    /// Gets or sets the unique identifier for this particular occurrence of the problem.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the links that provide additional information about the error.
    /// </summary>
    [JsonPropertyName("links")]
    public JsonApiErrorLinksObject? Links { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code associated with the error.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the application-specific error code associated with the error.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the title of the error.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the error.
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>
    /// Gets or sets the source of the error.
    /// </summary>
    [JsonPropertyName("source")]
    public JsonApiErrorSource? Source { get; set; }

    /// <summary>
    /// Gets or sets the additional metadata associated with the object.
    /// </summary>
    [JsonPropertyName("meta")]
    public JsonObject? Meta { get; set; }
}
