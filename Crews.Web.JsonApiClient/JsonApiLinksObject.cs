using Crews.Web.JsonApiClient.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a collection of links related to a resource or document as defined in section 7.6 of the JSON:API
/// speicification.
/// </summary>
public class JsonApiLinksObject
{
    /// <summary>
    /// Gets or sets the link to the current resource.
    /// </summary>
    [JsonPropertyName("self")]
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public JsonApiLink? Self { get; set; }

    /// <summary>
    /// Gets or sets a link to a related resource.
    /// </summary>
    [JsonPropertyName("related")]
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public JsonApiLink? Related { get; set; }

    /// <summary>
    /// Gets or sets a link to a resource that provides additional descriptive information about the current object.
    /// </summary>
    [JsonPropertyName("describedby")]
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public JsonApiLink? DescribedBy { get; set; }

    /// <summary>
    /// Gets or sets a link to the first page of a paginated collection.
    /// </summary>
    [JsonPropertyName("first")]
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public JsonApiLink? FirstPage { get; set; }

    /// <summary>
    /// Gets or sets the link to the last page of a paginated collection.
    /// </summary>
    [JsonPropertyName("last")]
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public JsonApiLink? LastPage { get; set; }

    /// <summary>
    /// Gets or sets the link to the previous page of a paginated collection.
    /// </summary>
    [JsonPropertyName("prev")]
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public JsonApiLink? PreviousPage { get; set; }

    /// <summary>
    /// Gets or sets the link to the next page of a paginated collection.
    /// </summary>
    [JsonPropertyName("next")]
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public JsonApiLink? NextPage { get; set; }

    /// <summary>
    /// Gets or sets additional link objects with non-standard names.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extensions { get; set; }
}
