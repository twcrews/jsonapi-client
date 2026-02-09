namespace Crews.Web.JsonApiClient.Utility;

static class Constants
{
	public static class Headers
	{
		public const string MediaType = "application/vnd.api+json";
		public const string ExtensionsParameterName = "ext";
		public const string ProfilesParameterName = "profile";
		public const string QualityParameterName = "q";
	}

	public static class Exceptions
	{
		public const string InvalidMediaType = 
			"Invalid media type; see https://jsonapi.org/format/#jsonapi-media-type";
		public const string InvalidHeaderParameters = 
			"Only `ext` and `profile` parameters are allowed; see https://jsonapi.org/format/#media-type-parameter-rules";
		public const string GetResourceInvalidType = 
			"Data is not an object; use GetResourceCollection if Data is an array";
		public const string GetResourceCollectionInvalidType = 
			"Data is not an array; use GetResource if Data is an object";
		public const string InvalidLinkJsonType = "Link must be a string or object";
    }
}
