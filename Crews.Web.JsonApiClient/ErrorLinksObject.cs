using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a set of links that provide additional information about an error in a JSON:API document.
/// </summary>
public class ErrorLinksObject
{
    /// <summary>
    /// Gets or sets a link that provides additional information about the error.
    /// </summary>
    public Link? About { get; set; }

    /// <summary>
    /// Gets or sets the link that specifies the type of the error.
    /// </summary>
    public Link? Type { get; set; }

    /// <summary>
    /// Gets or sets a collection of additional JSON properties that are not mapped to known members.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extensions { get; set; }
}
