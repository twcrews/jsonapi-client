using System.Text.Json;
using System.Text.Json.Nodes;

namespace Crews.Web.JsonApiClient.Tests;

public class JsonApiDocumentTests
{
	private readonly JsonSerializerOptions _options;

	public JsonApiDocumentTests()
	{
		_options = new JsonSerializerOptions();
	}

	// Concrete implementation for testing abstract JsonApiDocument
	private class TestJsonApiDocument : JsonApiDocument { }

	#region HasCollectionResource Tests

	[Fact(DisplayName = "HasCollectionResource returns true when Data is an array")]
	public void HasCollectionResourceReturnsTrueForArray()
	{
		const string json = """{"data": [{"type": "articles", "id": "1"}, {"type": "articles", "id": "2"}]}""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.True(doc.HasCollectionResource);
	}

	[Fact(DisplayName = "HasCollectionResource returns true when Data is an empty array")]
	public void HasCollectionResourceReturnsTrueForEmptyArray()
	{
		const string json = """{"data": []}""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.True(doc.HasCollectionResource);
	}

	[Fact(DisplayName = "HasCollectionResource returns false when Data is an object")]
	public void HasCollectionResourceReturnsFalseForObject()
	{
		const string json = """{"data": {"type": "articles", "id": "1"}}""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.False(doc.HasCollectionResource);
	}

	#endregion

	#region HasErrors Tests

	[Fact(DisplayName = "HasErrors returns true when Errors contains one or more errors")]
	public void HasErrorsReturnsTrueWhenErrorsPresent()
	{
		const string json = """
		{
			"errors": [
				{
					"status": "422",
					"title": "Invalid Attribute",
					"detail": "First name must contain at least two characters."
				}
			]
		}
		""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.True(doc.HasErrors);
	}

	[Fact(DisplayName = "HasErrors returns false when Errors is null")]
	public void HasErrorsReturnsFalseWhenErrorsNull()
	{
		const string json = """{"data": {"type": "articles", "id": "1"}}""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.Null(doc.Errors);
		Assert.False(doc.HasErrors);
	}

	[Fact(DisplayName = "HasErrors returns false when Errors is empty")]
	public void HasErrorsReturnsFalseWhenErrorsEmpty()
	{
		TestJsonApiDocument doc = new()
		{
			Errors = []
		};

		Assert.False(doc.HasErrors);
	}

	#endregion

	#region Property Deserialization Tests

	[Fact(DisplayName = "Deserializes document with JsonApi property")]
	public void DeserializesDocumentWithJsonApiProperty()
	{
		const string json = """
		{
			"jsonapi": {
				"version": "1.1",
				"meta": {
					"server": "my-server"
				}
			},
			"data": {"type": "articles", "id": "1"}
		}
		""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.NotNull(doc.JsonApi);
		Assert.Equal("1.1", doc.JsonApi.Version);
		Assert.NotNull(doc.JsonApi.Meta);
		Assert.Equal("my-server", doc.JsonApi.Meta["server"]!.GetValue<string>());
	}

	[Fact(DisplayName = "Deserializes document with Links property")]
	public void DeserializesDocumentWithLinksProperty()
	{
		const string json = """
		{
			"links": {
				"self": "https://example.com/articles"
			},
			"data": {"type": "articles", "id": "1"}
		}
		""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.NotNull(doc.Links);
		Assert.Equal("https://example.com/articles", doc.Links.First().Value.Href.OriginalString);
	}

	[Fact(DisplayName = "Deserializes document with Included property")]
	public void DeserializesDocumentWithIncludedProperty()
	{
		const string json = """
		{
			"data": {"type": "articles", "id": "1"},
			"included": [
				{"type": "people", "id": "9"},
				{"type": "comments", "id": "5"}
			]
		}
		""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.NotNull(doc.Included);
		JsonApiResource[] included = doc.Included.ToArray();
		Assert.Equal(2, included.Length);
		Assert.Equal("people", included[0].Type);
		Assert.Equal("9", included[0].Id);
		Assert.Equal("comments", included[1].Type);
		Assert.Equal("5", included[1].Id);
	}

	[Fact(DisplayName = "Deserializes document with Metadata property")]
	public void DeserializesDocumentWithMetadataProperty()
	{
		const string json = """
		{
			"meta": {
				"copyright": "Copyright 2024 Example Corp.",
				"authors": ["John Doe", "Jane Smith"]
			},
			"data": {"type": "articles", "id": "1"}
		}
		""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.NotNull(doc.Meta);
		Assert.Equal("Copyright 2024 Example Corp.", doc.Meta["copyright"]!.GetValue<string>());
		JsonArray? authors = doc.Meta["authors"]!.AsArray();
		Assert.NotNull(authors);
		Assert.Equal(2, authors.Count);
		Assert.Equal("John Doe", authors[0]!.GetValue<string>());
		Assert.Equal("Jane Smith", authors[1]!.GetValue<string>());
	}

	[Fact(DisplayName = "Deserializes document with Errors property")]
	public void DeserializesDocumentWithErrorsProperty()
	{
		const string json = """
		{
			"errors": [
				{
					"status": "403",
					"title": "Forbidden",
					"detail": "You do not have permission to access this resource."
				}
			]
		}
		""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.NotNull(doc.Errors);
		JsonApiError[] errors = doc.Errors.ToArray();
		Assert.Single(errors);
		Assert.Equal("403", errors[0].Status);
		Assert.Equal("Forbidden", errors[0].Title);
		Assert.Equal("You do not have permission to access this resource.", errors[0].Detail);
	}

	[Fact(DisplayName = "Deserializes document with Extensions property")]
	public void DeserializesDocumentWithExtensionsProperty()
	{
		const string json = """
		{
			"data": {"type": "articles", "id": "1"},
			"customExtension": {
				"foo": "bar",
				"count": 42
			}
		}
		""";

		TestJsonApiDocument? doc = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);

		Assert.NotNull(doc);
		Assert.NotNull(doc.Extensions);
		Assert.True(doc.Extensions.ContainsKey("customExtension"));
		JsonElement extension = doc.Extensions["customExtension"];
		Assert.Equal(JsonValueKind.Object, extension.ValueKind);
		Assert.Equal("bar", extension.GetProperty("foo").GetString());
		Assert.Equal(42, extension.GetProperty("count").GetInt32());
	}

	#endregion

	#region Serialization Tests

	[Fact(DisplayName = "Serializes document with single resource")]
	public void SerializesDocumentWithSingleResource()
	{
		TestJsonApiDocument doc = new()
		{
			Data = JsonSerializer.SerializeToElement(new JsonApiResource
			{
				Type = "articles",
				Id = "1",
				Attributes = new JsonObject
				{
					["title"] = "Test Article"
				}
			})
		};

		string json = JsonSerializer.Serialize(doc, _options);
		JsonDocument result = JsonDocument.Parse(json);

		Assert.Equal("articles", result.RootElement.GetProperty("data").GetProperty("type").GetString());
		Assert.Equal("1", result.RootElement.GetProperty("data").GetProperty("id").GetString());
		Assert.Equal("Test Article", result.RootElement.GetProperty("data").GetProperty("attributes").GetProperty("title").GetString());
	}

	[Fact(DisplayName = "Serializes document with resource collection")]
	public void SerializesDocumentWithResourceCollection()
	{
		JsonApiResource[] resources =
		[
			new JsonApiResource { Type = "articles", Id = "1" },
			new JsonApiResource { Type = "articles", Id = "2" }
		];

		TestJsonApiDocument doc = new()
		{
			Data = JsonSerializer.SerializeToElement(resources)
		};

		string json = JsonSerializer.Serialize(doc, _options);
		JsonDocument result = JsonDocument.Parse(json);

		JsonElement dataArray = result.RootElement.GetProperty("data");
		Assert.Equal(JsonValueKind.Array, dataArray.ValueKind);
		Assert.Equal(2, dataArray.GetArrayLength());
		Assert.Equal("articles", dataArray[0].GetProperty("type").GetString());
		Assert.Equal("1", dataArray[0].GetProperty("id").GetString());
		Assert.Equal("articles", dataArray[1].GetProperty("type").GetString());
		Assert.Equal("2", dataArray[1].GetProperty("id").GetString());
	}

	[Fact(DisplayName = "Serializes document with all properties")]
	public void SerializesDocumentWithAllProperties()
	{
		TestJsonApiDocument doc = new()
		{
			JsonApi = new JsonApiInfo { Version = "1.1" },
			Data = JsonSerializer.SerializeToElement(new JsonApiResource { Type = "articles", Id = "1" }),
			Links = new() { { "self", new JsonApiLink { Href = new("https://example.com/articles") } } },
			Included =
			[
				new JsonApiResource { Type = "people", Id = "9" }
			],
			Meta = new JsonObject { ["copyright"] = "2024" }
		};

		string json = JsonSerializer.Serialize(doc, _options);
		JsonDocument result = JsonDocument.Parse(json);

		Assert.Equal("1.1", result.RootElement.GetProperty("jsonapi").GetProperty("version").GetString());
		Assert.Equal("articles", result.RootElement.GetProperty("data").GetProperty("type").GetString());
		Assert.Equal("https://example.com/articles", result.RootElement.GetProperty("links").GetProperty("self").GetString());
		Assert.Equal("people", result.RootElement.GetProperty("included")[0].GetProperty("type").GetString());
		Assert.Equal("2024", result.RootElement.GetProperty("meta").GetProperty("copyright").GetString());
	}

	#endregion

	#region Roundtrip Tests

	[Fact(DisplayName = "Roundtrip serialization preserves single resource document")]
	public void RoundtripSerializationPreservesSingleResourceDocument()
	{
		const string json = """
		{
			"data": {
				"type": "articles",
				"id": "1",
				"attributes": {
					"title": "Test Article",
					"body": "Test content"
				}
			}
		}
		""";

		TestJsonApiDocument? original = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);
		Assert.NotNull(original);

		string serialized = JsonSerializer.Serialize(original, _options);
		TestJsonApiDocument? deserialized = JsonSerializer.Deserialize<TestJsonApiDocument>(serialized, _options);

		Assert.NotNull(deserialized);
		Assert.False(deserialized.HasCollectionResource);
		JsonApiResource? resource = JsonSerializer.Deserialize<JsonApiResource>((JsonElement)deserialized.Data!);
        Assert.NotNull(resource);
		Assert.Equal("articles", resource.Type);
		Assert.Equal("1", resource.Id);
		Assert.Equal("Test Article", resource.Attributes!["title"]!.GetValue<string>());
		Assert.Equal("Test content", resource.Attributes!["body"]!.GetValue<string>());
	}

	[Fact(DisplayName = "Roundtrip serialization preserves collection resource document")]
	public void RoundtripSerializationPreservesCollectionResourceDocument()
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

		TestJsonApiDocument? original = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);
		Assert.NotNull(original);

		string serialized = JsonSerializer.Serialize(original, _options);
		TestJsonApiDocument? deserialized = JsonSerializer.Deserialize<TestJsonApiDocument>(serialized, _options);

		Assert.NotNull(deserialized);
		Assert.True(deserialized.HasCollectionResource);
		IEnumerable<JsonApiResource>? resources = JsonSerializer.Deserialize<IEnumerable<JsonApiResource>>((JsonElement)deserialized.Data!);
        Assert.NotNull(resources);
		JsonApiResource[] resourceArray = resources.ToArray();
		Assert.Equal(3, resourceArray.Length);
		Assert.Equal("1", resourceArray[0].Id);
		Assert.Equal("2", resourceArray[1].Id);
		Assert.Equal("3", resourceArray[2].Id);
	}

	[Fact(DisplayName = "Roundtrip serialization preserves error document")]
	public void RoundtripSerializationPreservesErrorDocument()
	{
		const string json = """
		{
			"errors": [
				{
					"status": "422",
					"title": "Validation Error",
					"detail": "Name is required"
				}
			]
		}
		""";

		TestJsonApiDocument? original = JsonSerializer.Deserialize<TestJsonApiDocument>(json, _options);
		Assert.NotNull(original);

		string serialized = JsonSerializer.Serialize(original, _options);
		TestJsonApiDocument? deserialized = JsonSerializer.Deserialize<TestJsonApiDocument>(serialized, _options);

		Assert.NotNull(deserialized);
		Assert.True(deserialized.HasErrors);
		Assert.NotNull(deserialized.Errors);
		JsonApiError error = deserialized.Errors.First();
		Assert.Equal("422", error.Status);
		Assert.Equal("Validation Error", error.Title);
		Assert.Equal("Name is required", error.Detail);
	}

    #endregion

    #region Deserialize Static Method Tests

	[Fact(DisplayName = "Deserialize static method returns null for null JSON")]
	public void DeserializeStaticMethodReturnsNullForNullJson()
	{
		const string invalidJson = """null""";
		JsonApiDocument? doc = JsonApiDocument.Deserialize(invalidJson, _options);
		Assert.Null(doc);
    }

	[Fact(DisplayName = "Deserialize static method returns valid document for valid JSON")]
	public void DeserializeStaticMethodReturnsValidDocumentForValidJson()
    {
		const string validJson = """{"data": {"type": "articles", "id": "1"}}""";
		JsonApiDocument? doc = JsonApiDocument.Deserialize(validJson, _options);
		Assert.NotNull(doc);
		Assert.False(doc.HasCollectionResource);
    }

	[Fact(DisplayName = "Deserialize generic static method returns null for null JSON")]
	public void DeserializeGenericStaticMethodReturnsNullForNullJson()
    {
		const string invalidJson = """null""";
		JsonApiDocument<JsonApiResource>? doc = JsonApiDocument<JsonApiResource>.Deserialize(invalidJson, _options);
		Assert.Null(doc);
    }

	[Fact(DisplayName = "Deserialize generic static method returns valid document for valid JSON")]
	public void DeserializeGenericStaticMethodReturnsValidDocumentForValidJson()
    {
		const string validJson = """{"data": {"type": "articles", "id": "1"}}""";
		JsonApiDocument<JsonApiResource>? doc = JsonApiDocument<JsonApiResource>.Deserialize(validJson, _options);
		Assert.NotNull(doc);
		Assert.False(doc.HasCollectionResource);
    }

	[Fact(DisplayName = "DeserializeCollection generic static method returns null for null JSON")]
	public void DeserializeCollectionGenericStaticMethodReturnsNullForNullJson()
    {
		const string invalidJson = """null""";
		JsonApiCollectionDocument<JsonApiResource>? doc = JsonApiCollectionDocument<JsonApiResource>.Deserialize(invalidJson, _options);
		Assert.Null(doc);
    }

	[Fact(DisplayName = "DeserializeCollection generic static method returns valid document for valid JSON")]
	public void DeserializeCollectionGenericStaticMethodReturnsValidDocumentForValidJson()
    {
		const string validJson = """{"data": [{"type": "articles", "id": "1"}, {"type": "articles", "id": "2"}]}""";
		JsonApiCollectionDocument<JsonApiResource>? doc = JsonApiCollectionDocument<JsonApiResource>.Deserialize(validJson, _options);
		Assert.NotNull(doc);
		Assert.True(doc.HasCollectionResource);
    }

	public class MyModel
	{
		public string? Name { get; set; }
		public int Age { get; set; }
	}

	public class MyModelResource : JsonApiResource<MyModel> { }

	public class MyModelDocument : JsonApiDocument<MyModelResource> { }

    #endregion
}
