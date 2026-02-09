# `JsonApiClient`

A .NET client library for deserializing [JSON:API](https://jsonapi.org/) v1.1 documents into strongly-typed C# objects.

## Installation

`JsonApiClient` is available as a NuGet package:

```bash
dotnet add package Crews.Web.JsonApiClient
```

## Quick Start

```csharp
// Step 1: Define your base model
public class Article
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public DateTime? PublishedAt { get; set; }
}

// Step 2: Define a strongly-typed resource class extending JsonApiResource<T>
public class ArticleResource : JsonApiResource<Article> { }

// Step 3: Deserialize using the static Deserialize() method
string json = /* your JSON:API document */;
var document = JsonApiDocument<ArticleResource>.Deserialize(json);

// Step 4: Access strongly-typed data with full IntelliSense support!
if (document.HasErrors)
{
    foreach (var error in document.Errors)
    {
        Console.WriteLine($"Error {error.Status}: {error.Title}");
    }
}
else if (document.Data != null)
{
    // Data is strongly-typed as Article - get full IntelliSense!
    Console.WriteLine($"Title: {document.Data.Attributes?.Title}");
    Console.WriteLine($"Published: {document.Data.Attributes?.PublishedAt}");

    // Access typed relationships
    var authorId = document.Data.Relationships?.Author?.Data?.Id;
    Console.WriteLine($"Author ID: {authorId}");
}

// For collection documents, use JsonApiCollectionDocument<T>
var collection = JsonApiCollectionDocument<ArticleResource>.Deserialize(json);

if (collection.Data != null)
{
    foreach (var article in collection.Data)
    {
        Console.WriteLine($"Article: {article.Attributes?.Title}");
    }
}
```

### Weakly-Typed Deserialization (For Dynamic Schemas)

If you're working with dynamic or unknown schemas, you can use the weakly-typed base classes:

```csharp
// Deserialize without custom types
string json = /* your JSON:API document */;
var document = JsonApiDocument.Deserialize(json);

// Check what type of document you have
if (document.HasErrors)
{
    foreach (var error in document.Errors)
    {
        Console.WriteLine($"Error {error.Status}: {error.Title}");
    }
}
else if (document.HasCollectionResource)
{
    // Manually deserialize the Data property
    var resources = document.Data?.Deserialize<List<JsonApiResource>>();
    Console.WriteLine($"Found {resources?.Count} resources");
}
else
{
    // Single resource - manually deserialize the Data property
    var resource = document.Data?.Deserialize<JsonApiResource>();
    Console.WriteLine($"Resource: {resource?.Type} with ID {resource?.Id}");

    // Access attributes dynamically
    var title = resource?.Attributes?["title"]?.GetValue<string>();
    Console.WriteLine($"Title: {title}");
}
```

### Complete Real-World Example

Here's a complete example showing how to define and use strongly-typed resources:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using Crews.Web.JsonApiClient;

// Define your resource types
public class UserResource : JsonApiResource<User, UserRelationships> { }

public class User
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
}

public class UserRelationships
{
    [JsonPropertyName("posts")]
    public JsonApiCollectionRelationship<List<JsonApiResourceIdentifier>>? Posts { get; set; }

    [JsonPropertyName("profile")]
    public JsonApiRelationship<JsonApiResourceIdentifier>? Profile { get; set; }
}

// Use the types
string json = """
{
  "data": {
    "type": "users",
    "id": "123",
    "attributes": {
      "name": "John Doe",
      "email": "john@example.com",
      "createdAt": "2024-01-15T10:30:00Z"
    },
    "relationships": {
      "posts": {
        "data": [
          { "type": "posts", "id": "1" },
          { "type": "posts", "id": "2" }
        ]
      },
      "profile": {
        "data": { "type": "profiles", "id": "456" }
      }
    }
  }
}
""";

var document = JsonApiDocument<UserResource>.Deserialize(json);

// Access with full type safety and IntelliSense
if (document.Data != null)
{
    Console.WriteLine($"User: {document.Data.Attributes?.Name}");
    Console.WriteLine($"Email: {document.Data.Attributes?.Email}");
    Console.WriteLine($"Created: {document.Data.Attributes?.CreatedAt}");

    // Access typed relationships
    var posts = document.Data.Relationships?.Posts?.Data;
    Console.WriteLine($"Number of posts: {posts?.Count ?? 0}");

    var profileId = document.Data.Relationships?.Profile?.Data?.Id;
    Console.WriteLine($"Profile ID: {profileId}");
}
```

### Working with Resources

#### Strongly-Typed Approach (Recommended)

```csharp
// Use strongly-typed document with custom resource class
var document = JsonApiDocument<ArticleResource>.Deserialize(json);

// Access resource identification
Console.WriteLine($"Type: {document.Data?.Type}");
Console.WriteLine($"ID: {document.Data?.Id}");

// Access strongly-typed attributes with IntelliSense
if (document.Data?.Attributes != null)
{
    var title = document.Data.Attributes.Title;  // Full IntelliSense!
    var publishedAt = document.Data.Attributes.PublishedAt;  // Strongly-typed!
    Console.WriteLine($"{title} published at {publishedAt}");
}

// Access metadata (flexible JSON object for extension data)
if (document.Data?.Metadata != null)
{
    var copyright = document.Data.Metadata["copyright"]?.GetValue<string>();
    Console.WriteLine($"Copyright: {copyright}");
}

// Navigate links
if (document.Data?.Links?.Self != null)
{
    Console.WriteLine($"Self link: {document.Data.Links.Self.Href}");
}
```

#### Weakly-Typed Approach (For Dynamic Schemas)

```csharp
// Deserialize manually from Data property
var document = JsonApiDocument.Deserialize(json);
var resource = document.Data?.Deserialize<JsonApiResource>();

// Access resource identification
Console.WriteLine($"Type: {resource?.Type}");
Console.WriteLine($"ID: {resource?.Id}");

// Access attributes (flexible JSON object)
if (resource?.Attributes != null)
{
    var title = resource.Attributes["title"]?.GetValue<string>();
    var publishedAt = resource.Attributes["publishedAt"]?.GetValue<DateTime>();
    Console.WriteLine($"{title} published at {publishedAt}");
}

// Access metadata
if (resource?.Metadata != null)
{
    var copyright = resource.Metadata["copyright"]?.GetValue<string>();
    Console.WriteLine($"Copyright: {copyright}");
}

// Navigate links
if (resource?.Links?.Self != null)
{
    Console.WriteLine($"Self link: {resource.Links.Self.Href}");
}
```

### Working with Relationships

#### Strongly-Typed Approach (Recommended)

```csharp
var document = JsonApiDocument<ArticleResource>.Deserialize(json);

// Access strongly-typed relationships with IntelliSense
var authorRel = document.Data?.Relationships?.Author;
if (authorRel != null)
{
    // Data is strongly-typed as JsonApiResourceIdentifier
    Console.WriteLine($"Author: {authorRel.Data?.Type}/{authorRel.Data?.Id}");

    // Navigate relationship links
    if (authorRel.Links?.Related != null)
    {
        Console.WriteLine($"Fetch author at: {authorRel.Links.Related.Href}");
    }
}

// Access collection relationships (strongly-typed)
var commentsRel = document.Data?.Relationships?.Comments;
if (commentsRel?.Data != null)
{
    Console.WriteLine($"Comment count: {commentsRel.Data.Count}");
    foreach (var comment in commentsRel.Data)
    {
        Console.WriteLine($"Comment ID: {comment.Id}");
    }
}
```

#### Weakly-Typed Approach (For Dynamic Schemas)

```csharp
var document = JsonApiDocument.Deserialize(json);
var resource = document.Data?.Deserialize<JsonApiResource>();

// Access relationships dynamically
if (resource?.Relationships != null &&
    resource.Relationships.TryGetValue("author", out var authorRel))
{
    // Get related resource identifier
    var authorId = authorRel.Data?.Deserialize<JsonApiResourceIdentifier>();
    Console.WriteLine($"Author: {authorId?.Type}/{authorId?.Id}");

    // Navigate relationship links
    if (authorRel.Links?.Related != null)
    {
        Console.WriteLine($"Fetch author at: {authorRel.Links.Related.Href}");
    }
}
```

### Working with Included Resources

```csharp
// Find included resources
if (document.Included != null)
{
    var authors = document.Included.Where(r => r.Type == "authors");

    foreach (var author in authors)
    {
        var name = author.Attributes?["name"]?.GetValue<string>();
        Console.WriteLine($"Included author: {name}");
    }
}
```

### Handling Collections

#### Strongly-Typed Approach (Recommended)

```csharp
// Use strongly-typed collection document with custom resource class
var collection = JsonApiCollectionDocument<ArticleResource>.Deserialize(json);

if (collection.Data != null)
{
    foreach (var article in collection.Data)
    {
        // Access strongly-typed attributes with IntelliSense
        Console.WriteLine($"Article: {article.Attributes?.Title}");
        Console.WriteLine($"Published: {article.Attributes?.PublishedAt}");

        // Access relationships
        var authorId = article.Relationships?.Author?.Data?.Id;
        Console.WriteLine($"Author ID: {authorId}");
    }
}

// Access collection-level links (pagination)
if (collection.Links?.Next != null)
{
    Console.WriteLine($"Next page: {collection.Links.Next.Href}");
}
if (collection.Links?.Prev != null)
{
    Console.WriteLine($"Previous page: {collection.Links.Prev.Href}");
}
```

#### Weakly-Typed Approach (For Dynamic Schemas)

```csharp
// Deserialize collection manually
var document = JsonApiDocument.Deserialize(json);
var articles = document.Data?.Deserialize<List<JsonApiResource>>();

if (articles != null)
{
    foreach (var article in articles)
    {
        var title = article.Attributes?["title"]?.GetValue<string>();
        Console.WriteLine($"Article: {title}");
    }
}

// Access collection-level links
if (document.Links?.Next != null)
{
    Console.WriteLine($"Next page: {document.Links.Next.Href}");
}
```

### HTTP Client Integration

The library provides convenient extension methods for `HttpResponseMessage` that integrate seamlessly with `HttpClient`:

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using Crews.Web.JsonApiClient;

var client = new HttpClient();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/vnd.api+json")
);

// Strongly-typed collection - ReadJsonApiCollectionDocumentAsync<T>()
var response = await client.GetAsync("https://api.example.com/articles");
var collection = await response.ReadJsonApiCollectionDocumentAsync<ArticleResource>();

if (collection?.Data != null)
{
    foreach (var article in collection.Data)
    {
        Console.WriteLine($"Article: {article.Attributes?.Title}");
    }
}

// Strongly-typed single resource - ReadJsonApiDocumentAsync<T>()
var singleResponse = await client.GetAsync("https://api.example.com/articles/123");
var document = await singleResponse.ReadJsonApiDocumentAsync<ArticleResource>();

Console.WriteLine($"Title: {document?.Data?.Attributes?.Title}");

// Weakly-typed - ReadJsonApiDocumentAsync()
var weakResponse = await client.GetAsync("https://api.example.com/unknown");
var weakDoc = await weakResponse.ReadJsonApiDocumentAsync();

if (weakDoc?.HasErrors == true)
{
    foreach (var error in weakDoc.Errors!)
    {
        Console.WriteLine($"Error: {error.Title}");
    }
}
```

#### Using Custom Headers with Extensions and Profiles

```csharp
using Crews.Web.JsonApiClient.Utility;

// Build JSON:API content type header with extensions and profiles
var headerBuilder = new MediaTypeHeaderBuilder()
    .AddExtension(new Uri("https://example.com/ext/atomic"))
    .AddProfile(new Uri("https://example.com/profiles/flexible-pagination"));

var mediaType = headerBuilder.Build();

// Use with HttpClient
var client = new HttpClient();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue(mediaType.MediaType.ToString())
);

// Make request and deserialize in one step
var response = await client.GetAsync("https://api.example.com/articles");
var collection = await response.ReadJsonApiCollectionDocumentAsync<ArticleResource>();

// Access strongly-typed data
if (collection?.Data != null)
{
    foreach (var article in collection.Data)
    {
        Console.WriteLine($"Article: {article.Attributes?.Title}");
    }
}
```

#### Extension Methods Available

The library provides three extension methods on `HttpResponseMessage`:

1. **`ReadJsonApiDocumentAsync()`** - Deserializes to a weakly-typed `JsonApiDocument`
   ```csharp
   JsonApiDocument? doc = await response.ReadJsonApiDocumentAsync();
   ```

2. **`ReadJsonApiDocumentAsync<T>()`** - Deserializes to a strongly-typed `JsonApiDocument<T>` with a single resource
   ```csharp
   JsonApiDocument<ArticleResource>? doc = await response.ReadJsonApiDocumentAsync<ArticleResource>();
   ```

3. **`ReadJsonApiCollectionDocumentAsync<T>()`** - Deserializes to a strongly-typed `JsonApiCollectionDocument<T>` with a collection
   ```csharp
   JsonApiCollectionDocument<ArticleResource>? collection =
       await response.ReadJsonApiCollectionDocumentAsync<ArticleResource>();
   ```

All methods support:
- Optional `JsonSerializerOptions` for custom serialization behavior
- `CancellationToken` for cancellation support
- Automatic error document handling (errors deserialize naturally into `Errors` property)

### Error Handling

```csharp
if (document.HasErrors)
{
    foreach (var error in document.Errors)
    {
        Console.WriteLine($"Status: {error.Status}");
        Console.WriteLine($"Code: {error.Code}");
        Console.WriteLine($"Title: {error.Title}");
        Console.WriteLine($"Detail: {error.Detail}");

        // Error source information
        if (error.Source?.Pointer != null)
        {
            Console.WriteLine($"Source pointer: {error.Source.Pointer}");
        }

        // Error-specific links
        if (error.Links?.About != null)
        {
            Console.WriteLine($"More info: {error.Links.About.Href}");
        }
    }
}
```

### Complex Link Objects

```csharp
// Links can be simple strings or rich objects
var selfLink = resource.Links?.Self;

if (selfLink != null)
{
    Console.WriteLine($"URL: {selfLink.Href}");
    Console.WriteLine($"Relation: {selfLink.Rel}");
    Console.WriteLine($"Title: {selfLink.Title}");
    Console.WriteLine($"Media Type: {selfLink.Type}");

    // Language hints
    if (selfLink.HrefLang != null)
    {
        var languages = selfLink.HrefLang.GetValue<string[]>();
        Console.WriteLine($"Languages: {string.Join(", ", languages)}");
    }

    // Nested describedBy link
    if (selfLink.DescribedBy != null)
    {
        Console.WriteLine($"Described by: {selfLink.DescribedBy.Href}");
    }
}
```

### Serialization (Round-trip)

```csharp
// Create a document
var newDocument = new JsonApiDocument
{
    Data = JsonSerializer.SerializeToElement(new JsonApiResource
    {
        Type = "articles",
        Id = "123",
        Attributes = new JsonObject
        {
            ["title"] = "Hello World",
            ["body"] = "This is my first article"
        }
    })
};

// Serialize back to JSON
var json = JsonSerializer.Serialize(newDocument, new JsonSerializerOptions
{
    WriteIndented = true
});
```

## Features

- **Strongly-typed deserialization** - Define custom `JsonApiResource<T>` classes and get compile-time safety, IntelliSense, and refactoring support
- **HttpClient integration** - Extension methods for `HttpResponseMessage` (`ReadJsonApiDocumentAsync()`, `ReadJsonApiDocumentAsync<T>()`, `ReadJsonApiCollectionDocumentAsync<T>()`)
- **Simple static methods** - Use `JsonApiDocument<T>.Deserialize()` and `JsonApiCollectionDocument<T>.Deserialize()` for easy JSON parsing
- **Generic subclasses** for strongly-typed resources, relationships, and documents with full type safety
- **Dual typing approach** - Fall back to weakly-typed base classes for dynamic schemas when needed
- **Strongly-typed models** for all JSON:API specification elements
- **Flexible attribute storage** using `JsonObject` for dynamic schemas or strongly-typed classes for known schemas
- **Dual-format link support** (string URLs or rich link objects)
- **Extension support** via `[JsonExtensionData]` for custom JSON:API extensions
- **Helper methods** for safe document type checking (`HasErrors`, `HasCollectionResource`)
- **HTTP header utilities** for building spec-compliant Content-Type headers with extensions and profiles
- **.NET 8.0 target** with nullable reference types enabled

## Documentation

For more information about JSON:API, visit [jsonapi.org](https://jsonapi.org/).

## License

See [LICENSE](LICENSE) file for details.
