using System.Net;
using System.Text;
using System.Text.Json;

namespace Crews.Web.JsonApiClient.Tests;

public class HttpResponseMessageExtensionsTests
{
	private readonly JsonSerializerOptions _options;

	public HttpResponseMessageExtensionsTests()
	{
		_options = new JsonSerializerOptions();
	}

	private static HttpResponseMessage CreateResponse(string content)
	{
		return new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent(content, Encoding.UTF8, "application/vnd.api+json")
		};
	}

	[Fact(DisplayName = "ReadJsonApiDocumentAsync deserializes single resource document")]
	public async Task ReadJsonApiDocumentAsyncDeserializesSingleResourceDocument()
	{
		const string json = """{"data": {"type": "articles", "id": "1"}}""";
		using var response = CreateResponse(json);

		JsonApiDocument? doc = await response.ReadJsonApiDocumentAsync(_options);

		Assert.NotNull(doc);
		Assert.False(doc.HasCollectionResource);
	}

	[Fact(DisplayName = "ReadJsonApiDocumentAsync deserializes collection resource document")]
	public async Task ReadJsonApiDocumentAsyncDeserializesCollectionResourceDocument()
	{
		const string json = """{"data": [{"type": "articles", "id": "1"}, {"type": "articles", "id": "2"}]}""";
		using var response = CreateResponse(json);

		JsonApiDocument? doc = await response.ReadJsonApiDocumentAsync(_options);

		Assert.NotNull(doc);
		Assert.True(doc.HasCollectionResource);
	}

	[Fact(DisplayName = "ReadJsonApiDocumentAsync deserializes error document")]
	public async Task ReadJsonApiDocumentAsyncDeserializesErrorDocument()
	{
		const string json = """
		{
			"errors": [
				{
					"status": "404",
					"title": "Not Found",
					"detail": "The requested resource was not found."
				}
			]
		}
		""";
		using var response = CreateResponse(json);

		JsonApiDocument? doc = await response.ReadJsonApiDocumentAsync(_options);

		Assert.NotNull(doc);
		Assert.True(doc.HasErrors);
		JsonApiError error = doc.Errors!.First();
		Assert.Equal("404", error.Status);
		Assert.Equal("Not Found", error.Title);
	}

	[Fact(DisplayName = "ReadJsonApiDocumentAsync generic deserializes strongly-typed document")]
	public async Task ReadJsonApiDocumentAsyncGenericDeserializesStronglyTypedDocument()
	{
		const string json = """
		{
			"data": {
				"type": "articles",
				"id": "1",
				"attributes": {
					"title": "Test Article"
				}
			}
		}
		""";
		using var response = CreateResponse(json);

		JsonApiDocument<JsonApiResource>? doc = await response.ReadJsonApiDocumentAsync<JsonApiResource>(_options);

		Assert.NotNull(doc);
		Assert.NotNull(doc.Data);
		Assert.Equal("articles", doc.Data.Type);
		Assert.Equal("1", doc.Data.Id);
	}

	[Fact(DisplayName = "ReadJsonApiCollectionDocumentAsync deserializes collection document")]
	public async Task ReadJsonApiCollectionDocumentAsyncDeserializesCollectionDocument()
	{
		const string json = """
		{
			"data": [
				{"type": "articles", "id": "1"},
				{"type": "articles", "id": "2"},
				{"type": "articles", "id": "3"}
			]
		}
		""";
		using var response = CreateResponse(json);

		JsonApiCollectionDocument<JsonApiResource>? doc = await response.ReadJsonApiCollectionDocumentAsync<JsonApiResource>(_options);

		Assert.NotNull(doc);
		Assert.NotNull(doc.Data);
		JsonApiResource[] resources = doc.Data.ToArray();
		Assert.Equal(3, resources.Length);
		Assert.Equal("1", resources[0].Id);
		Assert.Equal("2", resources[1].Id);
		Assert.Equal("3", resources[2].Id);
	}

	[Fact(DisplayName = "ReadJsonApiDocumentAsync returns null for null JSON")]
	public async Task ReadJsonApiDocumentAsyncReturnsNullForNullJson()
	{
		const string json = """null""";
		using var response = CreateResponse(json);

		JsonApiDocument? doc = await response.ReadJsonApiDocumentAsync(_options);

		Assert.Null(doc);
	}

	[Fact(DisplayName = "ReadJsonApiDocumentAsync supports cancellation")]
	public async Task ReadJsonApiDocumentAsyncSupportsCancellation()
	{
		const string json = """{"data": {"type": "articles", "id": "1"}}""";
		using var response = CreateResponse(json);
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		await Assert.ThrowsAsync<TaskCanceledException>(
			() => response.ReadJsonApiDocumentAsync(_options, cts.Token));
	}

	[Fact(DisplayName = "ReadJsonApiDocumentAsync generic supports cancellation")]
	public async Task ReadJsonApiDocumentAsyncGenericSupportsCancellation()
	{
		const string json = """{"data": {"type": "articles", "id": "1"}}""";
		using var response = CreateResponse(json);
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		await Assert.ThrowsAsync<TaskCanceledException>(
			() => response.ReadJsonApiDocumentAsync<JsonApiResource>(_options, cts.Token));
	}

	[Fact(DisplayName = "ReadJsonApiCollectionDocumentAsync supports cancellation")]
	public async Task ReadJsonApiCollectionDocumentAsyncSupportsCancellation()
	{
		const string json = """{"data": [{"type": "articles", "id": "1"}]}""";
		using var response = CreateResponse(json);
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		await Assert.ThrowsAsync<TaskCanceledException>(
			() => response.ReadJsonApiCollectionDocumentAsync<JsonApiResource>(_options, cts.Token));
	}

	[Fact(DisplayName = "ReadJsonApiDocumentAsync deserializes document with all properties")]
	public async Task ReadJsonApiDocumentAsyncDeserializesDocumentWithAllProperties()
	{
		const string json = """
		{
			"jsonapi": {"version": "1.1"},
			"data": {"type": "articles", "id": "1"},
			"links": {"self": "https://example.com/articles/1"},
			"meta": {"copyright": "2024"}
		}
		""";
		using var response = CreateResponse(json);

		JsonApiDocument? doc = await response.ReadJsonApiDocumentAsync(_options);

		Assert.NotNull(doc);
		Assert.NotNull(doc.JsonApi);
		Assert.Equal("1.1", doc.JsonApi.Version);
		Assert.NotNull(doc.Links);
		Assert.NotNull(doc.Meta);
		Assert.Equal("2024", doc.Meta["copyright"]!.GetValue<string>());
	}
}
