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
    [JsonPropertyName("data")]
    public JsonElement? Data { get; set; }

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
    /// Gets or sets the <c>included</c> property of the document.
    /// </summary>
    [JsonPropertyName("included")]
    public IEnumerable<Resource>? Included { get; set; }

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
    /// Gets a value indicating whether the <see cref="Data"/> property contains a single resource object.
    /// </summary>
    /// <remarks>
    /// This property returns <see langword="true"/> if <see cref="Data"/> is a JSON object. No other validation or
    /// type checking is performed.
    /// </remarks>
    public bool HasSingleResource => Data?.ValueKind == JsonValueKind.Object;

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
    /// Attempts to deserialize the <see cref="Data"/> property as a <see cref="Resource"/> object.
    /// </summary>
    /// <returns>
    /// The deserialized <see cref="Resource"/> object if <see cref="Data"/> is a valid resource object, or
    /// <see langword="null"/> if <see cref="Data"/> is <see langword="null"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Resource? GetResource()
    {
        if (Data is null) return null;
        if (Data is JsonElement data && data.ValueKind == JsonValueKind.Object)
            return data.Deserialize<Resource>();

        throw new InvalidOperationException(Constants.Exceptions.GetResourceInvalidType);
    }

    /// <summary>
    /// Attempts to deserialize the <see cref="Data"/> property as a collection of <see cref="Resource"/> objects.
    /// </summary>
    /// <returns>
    /// The deserialized <see cref="Resource"/> collection if <see cref="Data"/> is a valid resource array, or
    /// <see langword="null"/> if <see cref="Data"/> is <see langword="null"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IEnumerable<Resource>? GetResourceCollection()
    {
        if (Data is null) return null;
        if (Data is JsonElement data && data.ValueKind == JsonValueKind.Array)
            return data.Deserialize<Resource[]>();

        throw new InvalidOperationException(Constants.Exceptions.GetResourceCollectionInvalidType);
    }
}
