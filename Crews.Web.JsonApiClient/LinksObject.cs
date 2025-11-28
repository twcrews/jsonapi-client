using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a collection of links related to a resource or document as defined in section 7.6 of the JSON:API
/// speicification.
/// </summary>
public class LinksObject
{
    /// <summary>
    /// Gets or sets the link to the current resource.
    /// </summary>
    [JsonPropertyName("self")]
    public Link? Self { get; set; }

    /// <summary>
    /// Gets or sets a link to a related resource.
    /// </summary>
    [JsonPropertyName("related")]
    public Link? Related { get; set; }

    /// <summary>
    /// Gets or sets a link to a resource that provides additional descriptive information about the current object.
    /// </summary>
    [JsonPropertyName("describedby")]
    public Link? DescribedBy { get; set; }

    /// <summary>
    /// Gets or sets a link to the first page of a paginated collection.
    /// </summary>
    [JsonPropertyName("first")]
    public Link? FirstPage { get; set; }

    /// <summary>
    /// Gets or sets the link to the last page of a paginated collection.
    /// </summary>
    [JsonPropertyName("last")]
    public Link? LastPage { get; set; }

    /// <summary>
    /// Gets or sets the link to the previous page of a paginated collection.
    /// </summary>
    [JsonPropertyName("prev")]
    public Link? PreviousPage { get; set; }

    /// <summary>
    /// Gets or sets the link to the next page of a paginated collection.
    /// </summary>
    [JsonPropertyName("next")]
    public Link? NextPage { get; set; }

    /// <summary>
    /// Gets or sets additional link objects with non-standard names.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extensions { get; set; }
}
