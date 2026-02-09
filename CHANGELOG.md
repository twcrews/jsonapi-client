# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - 2026-02-09

### Added

- Generic subclasses for strongly-typed deserialization:
  - `JsonApiDocument<T>` - Strongly-typed single resource document where `Data` is typed as `T?` (where `T : JsonApiResource`)
  - `JsonApiCollectionDocument<T>` - Strongly-typed collection document where `Data` is typed as `T?` (where `T : IEnumerable<JsonApiResource>`)
  - `JsonApiResource<T>` - Resource with strongly-typed `Attributes` property (typed as `T?` instead of `JsonObject?`)
  - `JsonApiResource<TAttributes, TRelationships>` - Resource with strongly-typed `Attributes` and `Relationships` properties
  - `JsonApiRelationship<T>` - Relationship with strongly-typed single resource identifier where `Data` is typed as `T?` (where `T : JsonApiResourceIdentifier`)
  - `JsonApiCollectionRelationship<T>` - Relationship with strongly-typed resource identifier collection where `Data` is typed as `T?` (where `T : IEnumerable<JsonApiResourceIdentifier>`)
- Static `Deserialize(string json, JsonSerializerOptions? options = null)` methods on all document classes for convenient JSON parsing
- Extension methods for `HttpResponseMessage.Content` to deserialize JSON:API documents directly from HTTP responses:
  - `ReadJsonApiDocumentAsync`
  - `ReadJsonApiDocumentAsync<T>`
  - `ReadJsonApiCollectionDocumentAsync<T>`

### Removed

- **Breaking change:** `GetResource()` method from `JsonApiDocument` (replaced by strongly-typed `JsonApiDocument<T>.Data` property or manual deserialization of `JsonApiDocument.Data`)
- **Breaking change:** `GetResourceCollection()` method from `JsonApiDocument` (replaced by strongly-typed `JsonApiCollectionDocument<T>.Data` property or manual deserialization of `JsonApiDocument.Data`)
- **Breaking change:** `HasSingleResource` method from `JsonApiDocument` (just use the inverse of `HasCollectionResource`)

## [2.0.0] - 2025-12-01

### Fixed

- **Breaking change:** The `Data` property in the `JsonApiRelationship` is now typed as `JsonElement?` instead of `object?`, consistent with the same property in the `JsonApiDocument` class.

## [1.0.0] - 2025-11-28

Initial release.

[3.0.0]: https://github.com/twcrews/jsonapi-client/compare/2.0.0...3.0.0
[2.0.0]: https://github.com/twcrews/jsonapi-client/compare/1.0.0...2.0.0
[1.0.0]: https://github.com/twcrews/jsonapi-client/releases/tag/1.0.0