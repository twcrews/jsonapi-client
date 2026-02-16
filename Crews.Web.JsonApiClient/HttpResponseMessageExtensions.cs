using System.Net.Http.Json;
using System.Text.Json;

namespace Crews.Web.JsonApiClient;

/// <summary>
/// Provides extension methods for <see cref="HttpResponseMessage"/> to deserialize JSON:API documents.
/// </summary>
public static class HttpResponseMessageExtensions
{
	/// <summary>
	/// Deserializes the HTTP response content as a weakly-typed JSON:API document.
	/// </summary>
	/// <param name="response">The HTTP response message.</param>
	/// <param name="options">Optional serialization options to control the deserialization behavior.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains a <see cref="JsonApiDocument"/>
	/// instance, or <see langword="null"/> if the response content is empty or invalid.
	/// </returns>
	public static Task<JsonApiDocument?> ReadJsonApiDocumentAsync(
		this HttpResponseMessage response,
		JsonSerializerOptions? options = null,
		CancellationToken cancellationToken = default)
		=> response.Content.ReadFromJsonAsync<JsonApiDocument>(options, cancellationToken);

	/// <summary>
	/// Deserializes the HTTP response content as a strongly-typed JSON:API document with a single resource.
	/// </summary>
	/// <typeparam name="T">The resource type, which must inherit from <see cref="JsonApiResource"/>.</typeparam>
	/// <param name="response">The HTTP response message.</param>
	/// <param name="options">Optional serialization options to control the deserialization behavior.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains a <see cref="JsonApiDocument{T}"/>
	/// instance, or <see langword="null"/> if the response content is empty or invalid.
	/// </returns>
	public static Task<JsonApiDocument<T>?> ReadJsonApiDocumentAsync<T>(
		this HttpResponseMessage response,
		JsonSerializerOptions? options = null,
		CancellationToken cancellationToken = default)
		where T : JsonApiResource
		=> response.Content.ReadFromJsonAsync<JsonApiDocument<T>>(options, cancellationToken);

	/// <summary>
	/// Deserializes the HTTP response content as a strongly-typed JSON:API document with a resource collection.
	/// </summary>
	/// <typeparam name="T">The resource type, which must inherit from <see cref="JsonApiResource"/>.</typeparam>
	/// <param name="response">The HTTP response message.</param>
	/// <param name="options">Optional serialization options to control the deserialization behavior.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains a <see cref="JsonApiCollectionDocument{T}"/>
	/// instance, or <see langword="null"/> if the response content is empty or invalid.
	/// </returns>
	public static Task<JsonApiCollectionDocument<T>?> ReadJsonApiCollectionDocumentAsync<T>(
		this HttpResponseMessage response,
		JsonSerializerOptions? options = null,
		CancellationToken cancellationToken = default)
		where T : JsonApiResource
		=> response.Content.ReadFromJsonAsync<JsonApiCollectionDocument<T>>(options, cancellationToken);
}
