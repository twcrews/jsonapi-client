# `JsonApiClient`

A .NET client library for deserializing [JSON:API](https://jsonapi.org/) v1.1 documents into strongly-typed C# objects.

## Installation

`JsonApiClient` is available as a NuGet package:

```bash
dotnet add package Crews.Web.JsonApiClient
```

## Quick Start

### Basic Deserialization

```csharp
using System.Text.Json;
using Crews.Web.JsonApiClient;

// Deserialize a JSON:API document
string json = /* your JSON:API document */;
var document = JsonSerializer.Deserialize<JsonApiDocument>(json);

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
    var resource = document.GetResource();
    Console.WriteLine($"Resource: {resource.Type} with ID {resource.Id}");
}
else if (document.HasCollectionResource)
{
    var resources = document.GetResourceCollection();
    Console.WriteLine($"Found {resources.Count()} resources");
}
```

### Working with Resources

```csharp
// Access resource identification
var resource = document.GetResource();
Console.WriteLine($"Type: {resource.Type}");
Console.WriteLine($"ID: {resource.Id}");

// Access attributes (flexible JSON object)
if (resource.Attributes != null)
{
    var title = resource.Attributes["title"]?.GetValue<string>();
    var publishedAt = resource.Attributes["publishedAt"]?.GetValue<DateTime>();
    Console.WriteLine($"{title} published at {publishedAt}");
}

// Access metadata
if (resource.Metadata != null)
{
    var copyright = resource.Metadata["copyright"]?.GetValue<string>();
    Console.WriteLine($"Copyright: {copyright}");
}

// Navigate links
if (resource.Links?.Self != null)
{
    Console.WriteLine($"Self link: {resource.Links.Self.Href}");
}
```

### Working with Relationships

```csharp
// Access relationships
if (resource.Relationships != null &&
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

```csharp
// Process a collection of resources
var articles = document.GetResourceCollection();

foreach (var article in articles)
{
    var title = article.Attributes?["title"]?.GetValue<string>();
    Console.WriteLine($"Article: {title}");

    // Access collection-level links
    if (document.Links?.Next != null)
    {
        Console.WriteLine($"Next page: {document.Links.Next.Href}");
    }
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

// Make request
var response = await client.GetAsync("https://api.example.com/articles");
var json = await response.Content.ReadAsStringAsync();
var document = JsonSerializer.Deserialize<JsonApiDocument>(json);
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

## Features

- **Strongly-typed models** for all JSON:API specification elements
- **Flexible attribute storage** using `JsonObject` for dynamic schemas
- **Dual-format link support** (string URLs or rich link objects)
- **Extension support** via `[JsonExtensionData]` for custom JSON:API extensions
- **Helper methods** for safe resource extraction and type checking
- **HTTP header utilities** for building spec-compliant Content-Type headers
- **.NET 8.0 target** with nullable reference types enabled

## Documentation

For more information about JSON:API, visit [jsonapi.org](https://jsonapi.org/).

## License

See [LICENSE](LICENSE) file for details.
