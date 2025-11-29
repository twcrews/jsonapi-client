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
dotnet test --filter "FullyQualifiedName~Crews.Web.JsonApiClient.Tests.JsonApiDocumentTests.HasSingleResourceReturnsTrueForObject"
```

## Architecture Overview

The library follows a **composition-based architecture** aligned with the JSON:API specification v1.1:

### Core Hierarchy

```
JsonApiDocument (abstract base)
├── Data (JsonElement?) - primary payload
├── Errors (IEnumerable<JsonApiError>?)
├── Included (IEnumerable<JsonApiResource>?)
├── Links (JsonApiLinksObject?)
├── Metadata (JsonObject?)
├── JsonApi (JsonApiInfo?)
└── Extensions (Dictionary<string, JsonElement>?)

JsonApiResource (extends JsonApiResourceIdentifier)
├── Type/Id/LocalId (identification)
├── Attributes (JsonObject?)
├── Relationships (Dictionary<string, JsonApiRelationship>?)
├── Links (JsonApiLinksObject?)
├── Metadata (JsonObject?)
└── Extensions (Dictionary<string, JsonElement>?)

JsonApiRelationship
├── Links (JsonApiLinksObject?)
├── Data (JsonElement?) - ResourceIdentifier or array
├── Metadata (JsonObject?)
└── Extensions (Dictionary<string, JsonElement>?)

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

1. **Abstract Base Class**: `JsonApiDocument` defines the contract for all JSON:API documents and provides helper methods:
   - `HasSingleResource` / `HasCollectionResource` / `HasErrors` - check document type
   - `GetResource()` / `GetResourceCollection()` - safe deserialization

2. **Dual-Format Serialization**: `JsonApiLinkConverter` handles JSON:API links, which can be either:
   - Simple strings: `"https://example.com"`
   - Complex objects: `{"href": "...", "rel": "self", "title": "...", "describedBy": {...}, "meta": {...}}`

3. **Fluent Builder**: `MediaTypeHeaderBuilder` constructs JSON:API-compliant HTTP headers with extensions and profiles

4. **Extension Points**: `[JsonExtensionData]` attributes enable JSON:API extensions without code changes

5. **Flexible Data Storage**: `JsonObject` and `JsonElement` used for Attributes, Metadata, and relationship Data to avoid premature schema commitments

6. **Nullable Reference Types**: All properties properly annotated with nullable reference types for compile-time null safety

### Data Flow

```
Raw JSON:API Response
    ↓ (System.Text.Json deserializes)
JsonApiDocument instance
    ↓ (check HasErrors, HasSingleResource, HasCollectionResource)
JsonApiResource object(s)
    ├── Access Attributes (JsonObject for flexible schema)
    ├── Follow Relationships (to other resources via JsonApiRelationship)
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
  - Helper methods (`HasSingleResource`, `HasCollectionResource`, `HasErrors`)
  - Resource extraction methods (`GetResource()`, `GetResourceCollection()`)
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
├── Constants.cs                            # Media types, parameters, exception messages
├── Converters/
│   └── JsonApiLinkConverter.cs             # Custom converter for link string/object duality
└── Utility/
    └── MediaTypeHeaderBuilder.cs           # Fluent builder for JSON:API Accept/Content-Type headers

Crews.Web.JsonApiClient.Tests/              # Test project (.NET 10.0)
├── JsonApiDocumentTests.cs                 # Comprehensive tests for JsonApiDocument (31 tests)
├── Converters/
│   └── JsonApiLinkConverterTests.cs        # Tests for link converter
├── Utility/
│   └── MediaTypeHeaderBuilderTests.cs      # Tests for header builder
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

The library has comprehensive test coverage across all major components:

- **JsonApiDocumentTests.cs**: 31 tests covering all aspects of the document model
  - HasSingleResource, HasCollectionResource, HasErrors property tests
  - GetResource() and GetResourceCollection() method tests
  - Property deserialization (JsonApi, Links, Included, Metadata, Errors, Extensions)
  - Serialization and roundtrip tests for all document types
- **JsonApiLinkConverterTests.cs**: Tests for dual-format link serialization
- **MediaTypeHeaderBuilderTests.cs**: Tests for fluent header construction with extensions and profiles
