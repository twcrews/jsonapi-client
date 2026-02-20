using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Represents a resource object as defined in section 7.2 of the JSON:API specification.
/// </summary>
public record JsonApiResource : JsonApiResourceIdentifier
{
    /// <summary>
    /// Gets or sets the collection of custom attributes associated with this object.
    /// </summary>
    [JsonPropertyName("attributes")]
    public JsonObject? Attributes { get; init; }

    /// <summary>
    /// Gets or sets the collection of relationships associated with this object.
    /// </summary>
    [JsonPropertyName("relationships")]
    public Dictionary<string, JsonApiRelationship>? Relationships { get; init; }

    /// <summary>
    /// Gets or sets the <c>links</c> property of the resource object.
    /// </summary>
    [JsonPropertyName("links")]
    public Dictionary<string, JsonApiLink>? Links { get; init; }

    /// <summary>
    /// Gets or sets the custom metadata associated with this object.
    /// </summary>
    [JsonPropertyName("meta")]
    public JsonObject? Meta { get; init; }
}

/// <summary>
/// Represents a resource object with a generic <see cref="Attributes"/> type as defined in section 7.2 of the JSON:API
/// specification.
/// </summary>
/// <typeparam name="T">The type of the <see cref="Attributes"/> property.</typeparam>
public record JsonApiResource<T> : JsonApiResource
{
    /// <summary>
    /// Gets or sets the collection of custom attributes associated with this object.
    /// </summary>
    public new T? Attributes { get; init; }
}

/// <summary>
/// Represents a JSON:API resource object with customizable attributes and relationships.
/// </summary>
/// <typeparam name="TAttributes">The type used to represent the attributes of the resource object.</typeparam>
/// <typeparam name="TRelationships">The type used to represent the relationships associated with the resource object.</typeparam>
public record JsonApiResource<TAttributes, TRelationships> : JsonApiResource<TAttributes>
{
    /// <summary>
    /// Gets or sets the collection of relationships associated with this object.
    /// </summary>
    [JsonPropertyName("relationships")]
    public new TRelationships? Relationships { get; init; }
}