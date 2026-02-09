# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **.NET C# library** that provides data structures and utilities for working with **JSON:API** (https://jsonapi.org/), a specification for building RESTful APIs. The library is named "netjac" (NET JSON:API Client) and acts as a client-side deserialization framework that maps JSON:API responses into strongly-typed C# objects.

**Target Framework**: .NET 8.0
**Test Framework**: xUnit
**JSON Serialization**: System.Text.Json (built-in)

## Common Commands

### Build
```bash
dotnet build Crews.Web.JsonApiClient.sln
```

### Run All Tests
```bash
dotnet test Crews.Web.JsonApiClient.sln
```

### Run Tests with Coverage
```bash
dotnet test Crews.Web.JsonApiClient.Tests/Crews.Web.JsonApiClient.Tests.csproj --settings Crews.Web.JsonApiClient.Tests/.runsettings --collect:"XPlat Code Coverage"
```
The `.runsettings` file configures code coverage to output in LCOV format.

### Run a Single Test Class
```bash
dotnet test --filter "FullyQualifiedName~Crews.Web.JsonApiClient.Tests.Converters.JsonApiLinkConverterTests"
```

### Run a Single Test Method
```bash
dotnet test --filter "FullyQualifiedName~Crews.Web.JsonApiClient.Tests.JsonApiDocumentTests.HasCollectionResourceReturnsTrueForArray"
```

## Architecture Overview

The library follows a **composition-based architecture** aligned with the JSON:API specification v1.1:

### Core Hierarchy

```
JsonApiDocument (base class)
├── Data (JsonElement?) - primary payload
├── Errors (IEnumerable<JsonApiError>?)
├── Included (IEnumerable<JsonApiResource>?)
├── Links (JsonApiLinksObject?)
├── Metadata (JsonObject?)
├── JsonApi (JsonApiInfo?)
└── Extensions (Dictionary<string, JsonElement>?)
    ├── JsonApiDocument<T> - strongly-typed single resource document
    │   └── Data (JsonApiResource<T>?) - resource with attributes type T
    └── JsonApiCollectionDocument<T> - strongly-typed collection document
        └── Data (IEnumerable<JsonApiResource<T>>?) - collection of resources with attributes type T

JsonApiResource (extends JsonApiResourceIdentifier)
├── Type/Id/LocalId (identification)
├── Attributes (JsonObject?)
├── Relationships (Dictionary<string, JsonApiRelationship>?)
├── Links (JsonApiLinksObject?)
├── Metadata (JsonObject?)
└── Extensions (Dictionary<string, JsonElement>?)
    ├── JsonApiResource<T> - strongly-typed attributes
    │   └── Attributes (T?)
    └── JsonApiResource<TAttributes, TRelationships> - strongly-typed attributes and relationships
        ├── Attributes (TAttributes?)
        └── Relationships (TRelationships?)

JsonApiRelationship
├── Links (JsonApiLinksObject?)
├── Data (JsonElement?) - ResourceIdentifier or array
├── Metadata (JsonObject?)
└── Extensions (Dictionary<string, JsonElement>?)
    ├── JsonApiRelationship<T> - strongly-typed single resource identifier
    │   └── Data (T?) where T : JsonApiResourceIdentifier
    └── JsonApiCollectionRelationship<T> - strongly-typed identifier collection
        └── Data (T?) where T : IEnumerable<JsonApiResourceIdentifier>

JsonApiLink
├── Href (Uri) - required
├── Rel (string?)
├── DescribedBy (JsonApiLink?)
├── Title (string?)
├── Type (string?)
├── HrefLang (string or string[]?)
├── Metadata (JsonObject?)
└── Extensions (Dictionary<string, JsonElement>?)
```

### Key Design Patterns

1. **Base Class with Generic Subclasses**: `JsonApiDocument`, `JsonApiResource`, and `JsonApiRelationship` serve as flexible base classes using `JsonElement`/`JsonObject` for weakly-typed scenarios, while generic subclasses provide compile-time type safety:
   - `JsonApiDocument<T>` / `JsonApiCollectionDocument<T>` - strongly-typed document data
   - `JsonApiResource<T>` / `JsonApiResource<TAttributes, TRelationships>` - strongly-typed resource attributes and relationships
   - `JsonApiRelationship<T>` / `JsonApiCollectionRelationship<T>` - strongly-typed relationship data
   - `HasCollectionResource` / `HasErrors` - check document type
   - Static `Deserialize()` methods on all document types for easy JSON deserialization

2. **Dual-Format Serialization**: `JsonApiLinkConverter` handles JSON:API links, which can be either:
   - Simple strings: `"https://example.com"`
   - Complex objects: `{"href": "...", "rel": "self", "title": "...", "describedBy": {...}, "meta": {...}}`

3. **Fluent Builder**: `MediaTypeHeaderBuilder` constructs JSON:API-compliant HTTP headers with extensions and profiles

4. **Extension Points**: `[JsonExtensionData]` attributes enable JSON:API extensions without code changes

5. **Flexible Data Storage**: `JsonObject` and `JsonElement` used in base classes for Attributes, Metadata, and relationship Data to avoid premature schema commitments, with generic subclasses available when schema is known

6. **Nullable Reference Types**: All properties properly annotated with nullable reference types for compile-time null safety

### Data Flow

**Weakly-Typed Approach (flexible, schema-agnostic):**
```
Raw JSON:API Response
    ↓ (JsonApiDocument.Deserialize() or JsonSerializer.Deserialize<JsonApiDocument>())
JsonApiDocument instance (Data as JsonElement)
    ↓ (check HasErrors, HasCollectionResource)
    ↓ (manually deserialize Data property)
JsonApiResource object(s)
    ├── Access Attributes (JsonObject for flexible schema)
    ├── Follow Relationships (Dictionary<string, JsonApiRelationship>)
    ├── Navigate via Links (hypermedia via JsonApiLink)
    └── Read Metadata (JsonObject)
```

**Strongly-Typed Approach (compile-time safety):**
```
Raw JSON:API Response
    ↓ (JsonApiDocument.Deserialize<MyAttributes>() or JsonSerializer.Deserialize<JsonApiDocument<MyAttributes>>())
JsonApiDocument<MyAttributes> instance (Data as JsonApiResource<MyAttributes>)
    ↓ (check HasErrors)
JsonApiResource<MyAttributes> object
    ├── Access Attributes (MyAttributes with typed properties)
    ├── Follow Relationships (Dictionary<string, JsonApiRelationship>)
    ├── Navigate via Links (hypermedia via JsonApiLink)
    └── Read Metadata (JsonObject)
```

## Key Implementation Details

### JSON Serialization
- All properties use `[JsonPropertyName]` for camelCase mapping (per JSON:API spec)
- `[JsonExtensionData]` captures unknown properties for forward compatibility
- Leverages `JsonElement` for flexible, unstructured data
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)

### Required Properties
- `JsonApiResourceIdentifier.Type` - required per JSON:API spec
- `JsonApiResourceIdentifier.Id` or `JsonApiResourceIdentifier.LocalId` - at least one required for resource identification
- `JsonApiLink.Href` - required for all links

### Testing Approach
- Tests organized by feature area matching source structure (`Converters/`, `Utility/`)
- xUnit test framework with Visual Studio integration
- Code coverage via coverlet.msbuild and coverlet.collector (LCOV format via `.runsettings`)
- Tests target .NET 10.0 (while library targets .NET 8.0 for compatibility)
- Comprehensive test coverage for `JsonApiDocument` including:
  - All property deserialization and serialization
  - Helper methods (`HasCollectionResource`, `HasErrors`)
  - Static `Deserialize()` methods on document classes
  - Generic subclass deserialization for strongly-typed scenarios
  - Roundtrip serialization tests
  - Extension data handling

## Project Structure

```
Crews.Web.JsonApiClient/                    # Main library (.NET 8.0)
├── JsonApiDocument.cs                      # Abstract base class for all JSON:API documents
├── JsonApiResource.cs                      # Resource objects with attributes, relationships
├── JsonApiResourceIdentifier.cs            # Type/Id identification for resources
├── JsonApiRelationship.cs                  # Resource relationships
├── JsonApiLink.cs                          # Link objects (href, metadata, etc.)
├── JsonApiLinksObject.cs                   # Collection of named links (self, related, etc.)
├── JsonApiError.cs                         # Error objects
├── JsonApiErrorSource.cs                   # Source pointer for errors
├── JsonApiErrorLinksObject.cs              # Links specific to error objects
├── JsonApiInfo.cs                          # jsonapi member (version, meta)
├── Converters/
│   └── JsonApiLinkConverter.cs             # Custom converter for link string/object duality
└── Utility/
    ├── Constants.cs                        # Media types, parameters, exception messages
    └── MediaTypeHeaderBuilder.cs           # Fluent builder for JSON:API Accept/Content-Type headers

Crews.Web.JsonApiClient.Tests/              # Test project (.NET 10.0)
├── JsonApiDocumentTests.cs                 # Comprehensive tests for JsonApiDocument (24 tests)
├── Converters/
│   └── JsonApiLinkConverterTests.cs        # Tests for link converter (16 tests)
├── Utility/
│   └── MediaTypeHeaderBuilderTests.cs      # Tests for header builder (9 tests)
├── GlobalSuppressions.cs                   # Code analysis suppressions
└── .runsettings                            # Test execution configuration (LCOV coverage)
```

## Development Notes

- The project uses implicit usings and file-scoped namespaces (C# 10+ features)
- Documentation XML generation is enabled for the main library (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`)
- All core classes use XML documentation comments for IntelliSense support
- Main branch is `master`
- All classes follow the `JsonApi*` naming convention for clarity and consistency
- Dependencies:
  - Main library: `Microsoft.Net.Http.Headers` (v8.0.22) for media type header handling
  - Test project: xUnit (v2.9.3), coverlet (v6.0.4), Microsoft.NET.Test.Sdk (v18.0.1)

## Current Test Coverage

The library has comprehensive test coverage across all major components (49 total tests):

- **JsonApiDocumentTests.cs**: 24 tests covering all aspects of the document model
  - HasCollectionResource, HasErrors property tests
  - Static Deserialize() method tests
  - Property deserialization (JsonApi, Links, Included, Metadata, Errors, Extensions)
  - Serialization and roundtrip tests for all document types
- **JsonApiLinkConverterTests.cs**: 16 tests for dual-format link serialization
- **MediaTypeHeaderBuilderTests.cs**: 9 tests for fluent header construction with extensions and profiles

## Changes in `dev` Branch (vs. `master`)

The `dev` branch introduces **generic subclasses** that enable strongly-typed deserialization while maintaining backward compatibility with the weakly-typed base classes:

### New Generic Classes

1. **JsonApiDocument<T>** - Strongly-typed single resource document
   - `Data` property is typed as `JsonApiResource<T>?` where `T` is the attributes type
   - Includes static `Deserialize<T>()` method on base class for easy JSON parsing
   - Example: `JsonApiDocument.Deserialize<UserAttributes>(json)` returns `JsonApiDocument<UserAttributes>`

2. **JsonApiCollectionDocument<T>** - Strongly-typed collection document
   - `Data` property is typed as `IEnumerable<JsonApiResource<T>>?` where `T` is the attributes type
   - Includes static `DeserializeCollection<T>()` method on base class
   - Example: `JsonApiDocument.DeserializeCollection<UserAttributes>(json)` returns `JsonApiCollectionDocument<UserAttributes>`

3. **JsonApiResource<T>** - Resource with strongly-typed attributes
   - `Attributes` property is typed as `T?` instead of `JsonObject?`
   - Example: Define `class UserResource : JsonApiResource<UserAttributes>`

4. **JsonApiResource<TAttributes, TRelationships>** - Resource with strongly-typed attributes and relationships
   - `Attributes` property is typed as `TAttributes?`
   - `Relationships` property is typed as `TRelationships?` instead of `Dictionary<string, JsonApiRelationship>?`
   - Example: `class UserResource : JsonApiResource<UserAttributes, UserRelationships>`

5. **JsonApiRelationship<T>** - Relationship with strongly-typed single resource identifier
   - `Data` property is typed as `T?` where `T : JsonApiResourceIdentifier`
   - Example: Used in relationship objects for to-one relationships

6. **JsonApiCollectionRelationship<T>** - Relationship with strongly-typed resource identifier collection
   - `Data` property is typed as `T?` where `T : IEnumerable<JsonApiResourceIdentifier>`
   - Example: Used in relationship objects for to-many relationships

### API Changes

**Removed Methods** (from `JsonApiDocument`):
- `GetResource()` - Previously used to deserialize `Data` as a single resource
- `GetResourceCollection()` - Previously used to deserialize `Data` as a resource collection

**Added Methods**:
- `JsonApiDocument.Deserialize(string json, JsonSerializerOptions? options = null)` - Static deserialization (weakly-typed)
- `JsonApiDocument.Deserialize<T>(string json, JsonSerializerOptions? options = null) where T : JsonApiResource` - Strongly-typed static deserialization
- `JsonApiDocument.DeserializeCollection<T>(string json, JsonSerializerOptions? options = null)` - Strongly-typed collection deserialization

### Migration Guide (master → dev)

**Before (master branch - weakly-typed):**
```csharp
var doc = JsonSerializer.Deserialize<JsonApiDocument>(json);
var resource = doc.GetResource();
var userName = resource?.Attributes?["userName"]?.GetString();
```

**After (dev branch - strongly-typed option):**
```csharp
var doc = JsonApiDocument.Deserialize<UserAttributes>(json);
var userName = doc.Data?.Attributes?.UserName;
```

**Or continue using weakly-typed approach:**
```csharp
var doc = JsonApiDocument.Deserialize(json);
var resource = doc.Data?.Deserialize<JsonApiResource>();
var userName = resource?.Attributes?["userName"]?.GetString();
```

### Benefits of Generic Subclasses

- **Compile-time type safety**: Catch errors at compile time instead of runtime
- **IntelliSense support**: Auto-completion for properties on typed attributes and relationships
- **Refactoring support**: IDE can track property renames and updates
- **Backward compatibility**: Base classes remain unchanged, existing code continues to work
- **Opt-in**: Use generics only when beneficial; fall back to flexible `JsonObject`/`JsonElement` when schema is unknown
