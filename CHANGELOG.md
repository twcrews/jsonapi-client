# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [5.0.0] - 2026-02-20

### Changed

- **Breaking change:** All model types have been changed from `class`es to `record`s.
- **Breaking change:** All property accessors on model types have changed from `{ get; set; }` to `{ get; init; }`.

### Removed

- **Breaking change:** `JsonApiLinksObject` has been removed.

### Remarks

This version refactors the entire model into immutable `record` types. While opinionated, I'm convinced this change is in the best interest of creating robust and resilient code.

If you're not willing to part with mutable `class`es, this version is otherwise identical to version 4.0.0.

## [4.0.0] - 2026-02-16

### Changed

- **Breaking change:** All `Links` property types are now `Dictionary<string, JsonApiLink>`.
- **Breaking change:** All property names now match their names in the JSON:API specification, with the exception of remaining pascal-cased.
  - All `Metadata` properties have been renamed to `Meta`.
  - `JsonApiError.StatusCode` has been renamed to `Status`.
  - `JsonApiError.ErrorCode` has been renamed to `Code`.
  - `JsonApiError.Details` has been renamed to `Detail`.
  - `JsonApiInfo.Extensions` has been renamed to `Ext`.
  - `JsonApiInfo.Profiles` has been renamed to `Profile`.
  - `JsonApiLink.HrefLanguage` has been renamed to `HrefLang`.
  - `JsonApiResourceIdentifier.LocalId` has been renamed to `LId`.

### Removed

- **Breaking change:** `JsonApiLinksObject` has been removed.

### Remarks

This version primarily fixes a critical deviation from the JSON:API specification in which `links` objects inside `relationships` objects were incorrectly typed as arrays, leading to deserialization exceptions. In the process of fixing this bug, the entire `JsonApiLinksObject` was removed, as it was foreign to the JSON:API specification for links.

Additionally, this version aims to be more idiomatic by renaming class properties to match their serialized representations.

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

### Changed

- **Breaking change:** The `Constants` class has been moved to the `Crews.Web.JsonApiClient.Utility` namespace.

### Removed

- **Breaking change:** `GetResource()` method from `JsonApiDocument` (replaced by strongly-typed `JsonApiDocument<T>.Data` property or manual deserialization of `JsonApiDocument.Data`)
- **Breaking change:** `GetResourceCollection()` method from `JsonApiDocument` (replaced by strongly-typed `JsonApiCollectionDocument<T>.Data` property or manual deserialization of `JsonApiDocument.Data`)
- **Breaking change:** `HasSingleResource` method from `JsonApiDocument` (just use the inverse of `HasCollectionResource`)

## [2.0.0] - 2025-12-01

### Fixed

- **Breaking change:** The `Data` property in the `JsonApiRelationship` is now typed as `JsonElement?` instead of `object?`, consistent with the same property in the `JsonApiDocument` class.

## [1.0.0] - 2025-11-28

Initial release.

[5.0.0]: https://github.com/twcrews/jsonapi-client/compare/4.0.0...5.0.0
[4.0.0]: https://github.com/twcrews/jsonapi-client/compare/3.0.0...4.0.0
[3.0.0]: https://github.com/twcrews/jsonapi-client/compare/2.0.0...3.0.0
[2.0.0]: https://github.com/twcrews/jsonapi-client/compare/1.0.0...2.0.0
[1.0.0]: https://github.com/twcrews/jsonapi-client/releases/tag/1.0.0