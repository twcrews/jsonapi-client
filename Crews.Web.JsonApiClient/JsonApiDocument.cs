using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a base class for JSON:API top-level objects as defined in section 7.1 of the JSON:API specification.
/// </summary>
public class JsonApiDocument
{
    /// <summary>
    /// Gets or sets the <c>jsonapi</c> property of the document.
    /// </summary>
    [JsonPropertyName("jsonapi")]
    public JsonApiInfo? JsonApi { get; set; }

    /// <summary>
    /// Gets or sets the primary data payload associated with the document.
    /// </summary>
    [JsonPropertyName("data")]
    public JsonElement? Data { get; set; }

    /// <summary>
    /// Gets or sets the collection of errors associated with the document.
    /// </summary>
    [JsonPropertyName("errors")]
    public IEnumerable<JsonApiError>? Errors { get; set; }

    /// <summary>
    /// Gets or sets the <c>links</c> property of the document.
    /// </summary>
    /// <seealso href="https://jsonapi.org/format/#document-links"/>
    [JsonPropertyName("links")]
    public JsonApiLinksObject? Links { get; set; }

    /// <summary>
    /// Gets or sets the <c>included</c> property of the document.
    /// </summary>
    [JsonPropertyName("included")]
    public IEnumerable<JsonApiResource>? Included { get; set; }

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

    /// <summary>
    /// Gets a value indicating whether the <see cref="Data"/> property contains a resource collection object.
    /// </summary>
    /// <remarks>
    /// This property returns <see langword="true"/> if <see cref="Data"/> is a JSON array. No other validation or
    /// type checking is performed.
    /// </remarks>
    public bool HasCollectionResource => Data?.ValueKind == JsonValueKind.Array;

    /// <summary>
    /// Gets a value indicating whether the <see cref="Errors"/> property contains one or more objects.
    /// </summary>
    public bool HasErrors => Errors is not null && Errors.Any();

    /// <summary>
    /// Deserializes the specified JSON string into a <see cref="JsonApiDocument"/> instance.
    /// </summary>
    /// <remarks>This method uses <see cref="JsonSerializer"/> for deserialization. The input
    /// JSON must conform to the JSON:API specification for successful parsing.</remarks>
    /// <param name="json">The JSON string representing a JSON:API document to deserialize.</param>
    /// <param name="options">Optional serialization options to control the deserialization behavior.</param>
    /// <returns>
    /// A <see cref="JsonApiDocument"/> instance representing the deserialized data, or <see langword="null"/> if the
    /// input is invalid or does not match the expected format.
    /// </returns>
    public static JsonApiDocument? Deserialize(string json, JsonSerializerOptions? options = null)
        => JsonSerializer.Deserialize<JsonApiDocument>(json, options);

    /// <summary>
    /// Deserializes the specified JSON string into a JSON:API document with a user-defined data object.
    /// </summary>
    /// <remarks>This method uses <see cref="JsonSerializer"/> for deserialization. The input
    /// JSON must conform to the JSON:API specification for successful parsing.</remarks>
    /// <param name="json">The JSON string representing a JSON:API document to deserialize.</param>
    /// <param name="options">Optional serialization options to control the deserialization behavior.</param>
    /// <returns>
    /// A <see cref="JsonApiDocument{T}"/> instance representing the deserialized data, or <see langword="null"/> if
    /// the input is invalid or does not match the expected format.
    /// </returns>
    public static JsonApiDocument<T>? Deserialize<T>(string json, JsonSerializerOptions? options = null)
        where T : JsonApiResource
        => JsonSerializer.Deserialize<JsonApiDocument<T>>(json, options);

    /// <summary>
    /// Deserializes the specified JSON string into a JSON:API document with a user-defined collection of data objects.
    /// </summary>
    /// <typeparam name="T">The underlying type of each item in the collection.</typeparam>
    /// <param name="json">The JSON string representing a JSON:API document to deserialize.</param>
    /// <param name="options">Optional serialization options to control the deserialization behavior.</param>
    /// <returns>
    /// A <see cref="JsonApiDocument{T}"/> instance representing the deserialized data, or <see langword="null"/> if
    /// the input is invalid or does not match the expected format.
    /// </returns>
    public static JsonApiCollectionDocument<T>? DeserializeCollection<T>(
        string json, JsonSerializerOptions? options = null)
        => JsonSerializer.Deserialize<JsonApiCollectionDocument<T>>(json, options);
}

/// <summary>
/// Represents a JSON:API top-level object with a generic single resource type as defined in section 7.1 of the
/// JSON:API specification.
/// </summary>
/// <typeparam name="T">The underlying resource type.</typeparam>
public class JsonApiDocument<T> : JsonApiDocument
{
    /// <summary>
    /// Gets or sets the primary data payload associated with the document.
    /// </summary>
    [JsonPropertyName("data")]
    public new JsonApiResource<T>? Data { get; set; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Data"/> property contains a single resource object.
    /// </summary>
    public new bool HasCollectionResource => false;
}

/// <summary>
/// Represents a JSON:API top-level object with a generic collection resource type as defined in section 7.1 of the
/// JSON:API specification.
/// </summary>
/// <typeparam name="T">The underlying resource type.</typeparam>
public class JsonApiCollectionDocument<T> : JsonApiDocument
{
    /// <summary>
    /// Gets or sets the primary data payload associated with the document.
    /// </summary>
    [JsonPropertyName("data")]
    public new IEnumerable<JsonApiResource<T>>? Data { get; set; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Data"/> property contains a resource collection object.
    /// </summary>
    public new bool HasCollectionResource => true;
}