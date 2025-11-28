namespace Crews.Web.JsonApiClient;

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
		public const string IllegalMediaType = 
			"Invalid media type. See https://jsonapi.org/format/#jsonapi-media-type";
		public const string IllegalHeaderParameters = 
			"Only `ext` and `profile` parameters are allowed. See https://jsonapi.org/format/#media-type-parameter-rules";
	}
}
