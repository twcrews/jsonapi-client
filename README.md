# `JsonApiClient`

A .NET client library for deserializing [JSON:API](https://jsonapi.org/) v1.1 documents into strongly-typed C# objects.

## Installation

`JsonApiClient` is available as a NuGet package:

```bash
dotnet add package Crews.Web.JsonApiClient
```

## Quick Start

### Basic Deserialization (Weakly-Typed)

```csharp
using System.Text.Json;
using Crews.Web.JsonApiClient;

// Deserialize a JSON:API document
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
else if (document.HasSingleResource)
{
    // Manually deserialize the Data property
    var resource = document.Data?.Deserialize<JsonApiResource>();
    Console.WriteLine($"Resource: {resource?.Type} with ID {resource?.Id}");
}
else if (document.HasCollectionResource)
{
    // Manually deserialize the Data property
    var resources = document.Data?.Deserialize<List<JsonApiResource>>();
    Console.WriteLine($"Found {resources?.Count} resources");
}
```

### Strongly-Typed Deserialization

For better type safety and IntelliSense support, use the generic subclasses:

```csharp
// Define your strongly-typed resource
public class Article : JsonApiResource<ArticleAttributes, ArticleRelationships>
{
}

public class ArticleAttributes
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("publishedAt")]
    public DateTime? PublishedAt { get; set; }
}

public class ArticleRelationships
{
    [JsonPropertyName("author")]
    public JsonApiRelationship<JsonApiResourceIdentifier>? Author { get; set; }

    [JsonPropertyName("comments")]
    public JsonApiCollectionRelationship<List<JsonApiResourceIdentifier>>? Comments { get; set; }
}

// Deserialize a single resource document
string json = /* your JSON:API document */;
var document = JsonApiDocument<Article>.Deserialize(json);

if (document.HasErrors)
{
    foreach (var error in document.Errors)
    {
        Console.WriteLine($"Error {error.Status}: {error.Title}");
    }
}
else if (document.Data != null)
{
    // Data is strongly-typed as Article
    Console.WriteLine($"Title: {document.Data.Attributes?.Title}");
    Console.WriteLine($"Published: {document.Data.Attributes?.PublishedAt}");

    // Access typed relationships
    var authorId = document.Data.Relationships?.Author?.Data?.Id;
    Console.WriteLine($"Author ID: {authorId}");
}

// Deserialize a collection document
var collection = JsonApiCollectionDocument<List<Article>>.Deserialize(json);

if (collection.Data != null)
{
    foreach (var article in collection.Data)
    {
        Console.WriteLine($"Article: {article.Attributes?.Title}");
    }
}
```

### Complete Real-World Example

Here's a complete example showing how to define and use strongly-typed resources:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using Crews.Web.JsonApiClient;

// Define your resource types
public class User : JsonApiResource<UserAttributes, UserRelationships>
{
}

public class UserAttributes
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

var document = JsonApiDocument<User>.Deserialize(json);

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

#### Weakly-Typed Approach

```csharp
// Deserialize manually from Data property
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

#### Strongly-Typed Approach

```csharp
// Use strongly-typed document
var document = JsonApiDocument<Article>.Deserialize(json);

// Access resource identification
Console.WriteLine($"Type: {document.Data?.Type}");
Console.WriteLine($"ID: {document.Data?.Id}");

// Access strongly-typed attributes with IntelliSense
if (document.Data?.Attributes != null)
{
    var title = document.Data.Attributes.Title;
    var publishedAt = document.Data.Attributes.PublishedAt;
    Console.WriteLine($"{title} published at {publishedAt}");
}

// Access metadata (still flexible JSON object)
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

### Working with Relationships

#### Weakly-Typed Approach

```csharp
var resource = document.Data?.Deserialize<JsonApiResource>();

// Access relationships
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

#### Strongly-Typed Approach

```csharp
var document = JsonApiDocument<Article>.Deserialize(json);

// Access strongly-typed relationships
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

// Access collection relationships
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

#### Weakly-Typed Approach

```csharp
// Deserialize collection manually
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

#### Strongly-Typed Approach

```csharp
// Use strongly-typed collection document
var collection = JsonApiCollectionDocument<List<Article>>.Deserialize(json);

if (collection.Data != null)
{
    foreach (var article in collection.Data)
    {
        // Access strongly-typed attributes
        Console.WriteLine($"Article: {article.Attributes?.Title}");
    }
}

// Access collection-level links
if (collection.Links?.Next != null)
{
    Console.WriteLine($"Next page: {collection.Links.Next.Href}");
}
```

### HTTP Client Integration

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using Crews.Web.JsonApiClient.Utility;

// Build JSON:API content type header
var headerBuilder = new MediaTypeHeaderBuilder()
    .AddExtension(new Uri("https://example.com/ext/atomic"))
    .AddProfile(new Uri("https://example.com/profiles/flexible-pagination"));

var mediaType = headerBuilder.Build();

// Use with HttpClient
var client = new HttpClient();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue(mediaType.MediaType.ToString())
);

// Make request and deserialize (weakly-typed)
var response = await client.GetAsync("https://api.example.com/articles");
var json = await response.Content.ReadAsStringAsync();
var document = JsonApiDocument.Deserialize(json);

// Or use strongly-typed deserialization
var typedDocument = JsonApiCollectionDocument<List<Article>>.Deserialize(json);
```

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

## When to Use Which Approach

### Use Strongly-Typed (Generic Subclasses) When:

- You have a **known, stable schema** for your JSON:API resources
- You want **compile-time type safety** and catch errors early
- You need **IntelliSense/autocomplete** support in your IDE
- You're building a **client for a specific API** with well-defined resource types
- You want **refactoring support** (rename properties, find usages, etc.)

### Use Weakly-Typed (Base Classes) When:

- You're working with **dynamic or unknown schemas**
- The API schema **changes frequently** or varies by endpoint
- You're building **generic tooling** that works with any JSON:API endpoint
- You need **maximum flexibility** to handle diverse response structures
- You're **exploring an API** and don't want to define types upfront

### Mixing Both Approaches

You can mix both approaches in the same application:

```csharp
// Use strongly-typed for known resources
var articles = JsonApiCollectionDocument<List<Article>>.Deserialize(articlesJson);

// Use weakly-typed for dynamic/unknown resources
var unknownDoc = JsonApiDocument.Deserialize(dynamicJson);
var resource = unknownDoc.Data?.Deserialize<JsonApiResource>();
```

## Features

- **Dual typing approach** - Choose between weakly-typed (flexible, schema-agnostic) or strongly-typed (compile-time safety, IntelliSense) deserialization
- **Generic subclasses** for strongly-typed resources, relationships, and documents with full type safety
- **Static deserialization methods** on all document classes for convenient JSON parsing
- **Strongly-typed models** for all JSON:API specification elements
- **Flexible attribute storage** using `JsonObject` for dynamic schemas (or strongly-typed for known schemas)
- **Dual-format link support** (string URLs or rich link objects)
- **Extension support** via `[JsonExtensionData]` for custom JSON:API extensions
- **Helper methods** for safe document type checking (`HasErrors`, `HasSingleResource`, `HasCollectionResource`)
- **HTTP header utilities** for building spec-compliant Content-Type headers with extensions and profiles
- **.NET 8.0 target** with nullable reference types enabled
- **Backward compatible** - existing code continues to work with base classes

## Migration Guide (v2.0.0 → v3.0.0)

Version 3.0.0 removes the `GetResource()` and `GetResourceCollection()` methods in favor of strongly-typed generic subclasses and manual deserialization. Here's how to migrate:

### Before (v2.0.0)

```csharp
var document = JsonSerializer.Deserialize<JsonApiDocument>(json);

// Get single resource
var resource = document.GetResource();
var title = resource?.Attributes?["title"]?.GetValue<string>();

// Get collection
var resources = document.GetResourceCollection();
foreach (var resource in resources)
{
    // Process resource
}
```

### After (v3.0.0) - Option 1: Weakly-Typed

```csharp
var document = JsonApiDocument.Deserialize(json);

// Get single resource
var resource = document.Data?.Deserialize<JsonApiResource>();
var title = resource?.Attributes?["title"]?.GetValue<string>();

// Get collection
var resources = document.Data?.Deserialize<List<JsonApiResource>>();
if (resources != null)
{
    foreach (var resource in resources)
    {
        // Process resource
    }
}
```

### After (v3.0.0) - Option 2: Strongly-Typed (Recommended)

```csharp
// Define your types once
public class Article : JsonApiResource<ArticleAttributes>
{
}

public class ArticleAttributes
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

// Use strongly-typed deserialization
var document = JsonApiDocument<Article>.Deserialize(json);
var title = document.Data?.Attributes?.Title; // Full IntelliSense support!

// Or for collections
var collection = JsonApiCollectionDocument<List<Article>>.Deserialize(json);
if (collection.Data != null)
{
    foreach (var article in collection.Data)
    {
        Console.WriteLine(article.Attributes?.Title);
    }
}
```

## Documentation

For more information about JSON:API, visit [jsonapi.org](https://jsonapi.org/).

## License

See [LICENSE](LICENSE) file for details.
