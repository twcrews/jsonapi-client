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
dotnet test --filter "FullyQualifiedName~Crews.Web.JsonApiClient.Tests.Converters.LinkConverterTests"
```

### Run a Single Test Method
```bash
dotnet test --filter "FullyQualifiedName~Crews.Web.JsonApiClient.Tests.Converters.LinkConverterTests.TestMethodName"
```

## Architecture Overview

The library follows a **composition-based architecture** aligned with the JSON:API specification v1.1:

### Core Hierarchy

```
JsonApiDocument (abstract base)
├── Data (JsonElement) - primary payload
├── Errors (IEnumerable<Error>)
├── Included (IEnumerable<Resource>)
├── Links (LinksObject)
├── Metadata (JsonObject)
└── JsonApi (JsonApiInfo)

Resource (extends ResourceIdentifier)
├── Type/ID/LocalID (identification)
├── Attributes (JsonObject)
├── Relationships (Dictionary<string, Relationship>)
├── Links (LinksObject)
└── Metadata (JsonObject)

Relationship
├── Links (IEnumerable<Link>)
├── Data (ResourceIdentifier or array)
├── Metadata (JsonObject)
└── Extensions (Dictionary<string, JsonElement>)
```

### Key Design Patterns

1. **Abstract Base Class**: `JsonApiDocument` defines the contract for all JSON:API documents and provides helper methods:
   - `HasSingleResource` / `HasCollectionResource` / `HasErrors` - check document type
   - `GetResource()` / `GetResourceCollection()` - safe deserialization

2. **Dual-Format Serialization**: `LinkConverter` handles JSON:API links, which can be either:
   - Simple strings: `"https://example.com"`
   - Complex objects: `{"href": "...", "rel": "self", "title": "..."}`

3. **Fluent Builder**: `MediaTypeHeaderBuilder` constructs JSON:API-compliant HTTP headers with extensions and profiles

4. **Extension Points**: `[JsonExtensionData]` attributes enable JSON:API extensions without code changes

5. **Flexible Data Storage**: `JsonObject` and `JsonElement` used for Attributes and Metadata to avoid premature schema commitments

### Data Flow

```
Raw JSON:API Response
    ↓ (System.Text.Json deserializes)
JsonApiDocument instance
    ↓ (check HasErrors, HasSingleResource, HasCollectionResource)
Resource object(s)
    ├── Access Attributes (JsonObject for flexible schema)
    ├── Follow Relationships (to other resources)
    ├── Navigate via Links (hypermedia)
    └── Read Metadata
```

## Key Implementation Details

### JSON Serialization
- All properties use `[JsonPropertyName]` for camelCase mapping (per JSON:API spec)
- `[JsonExtensionData]` captures unknown properties for forward compatibility
- Leverages `JsonElement` for flexible, unstructured data
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)

### Required Properties
- `ResourceIdentifier.Type` - required per JSON:API spec
- `ResourceIdentifier.ID` or `ResourceIdentifier.LocalID` - at least one required for resource identification
- `Link.Href` - required for all links

### Testing Approach
- Tests organized by feature area matching source structure (`Converters/`, `Utility/`)
- xUnit test framework with Visual Studio integration
- Code coverage via coverlet.msbuild and coverlet.collector
- Tests target .NET 10.0 (while library targets .NET 8.0 for compatibility)

## Project Structure

```
Crews.Web.JsonApiClient/            # Main library
├── Core Domain Models              # JsonApiDocument, Resource, ResourceIdentifier, etc.
├── Error Models                    # Error, ErrorSource, ErrorLinksObject
├── Supporting Models               # JsonApiInfo, LinksObject
├── Converters/                     # Custom JSON converters (LinkConverter)
├── Utility/                        # Helper classes (MediaTypeHeaderBuilder)
└── Constants.cs                    # Media types, parameters, exception messages

Crews.Web.JsonApiClient.Tests/      # Test project
├── Converters/                     # Tests for custom converters
├── Utility/                        # Tests for utility classes
├── GlobalSuppressions.cs           # Code analysis suppressions
└── .runsettings                    # Test execution configuration
```

## Development Notes

- The library is in active development - JsonApiDocumentTests.cs currently has no test implementations
- The project uses implicit usings and file-scoped namespaces (C# 10+ features)
- Documentation XML generation is enabled for the main library (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`)
- Main branch is `master`
