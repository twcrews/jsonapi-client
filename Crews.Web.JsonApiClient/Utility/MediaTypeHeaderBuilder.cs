using Microsoft.Net.Http.Headers;

namespace Crews.Web.JsonApiClient.Utility;

/// <summary>
/// A class used to build <c>Content-Type</c> headers conforming to section 5 of the JSON:API specification.
/// </summary>
public class MediaTypeHeaderBuilder
{
	private static readonly string[] _legalParameterNames = 
	[
		Constants.Headers.ExtensionsParameterName, 
		Constants.Headers.ProfilesParameterName, 
		Constants.Headers.QualityParameterName
	];

	/// <summary>
	/// Extension URIs appended to the extensions parameter of the header as defined by section 5.4 of the JSON:API 
	/// specification.
	/// </summary>
	public HashSet<Uri> Extensions { get; set; } = [];

	/// <summary>
	/// Profile URIs appended to the profiles parameter of the header as defined by section 5.4 of the JSON:API
	/// specification.
	/// </summary>
	public HashSet<Uri> Profiles { get; set; } = [];

  /// <summary>
	/// Creates a new builder instance using an existing header.
	/// </summary>
	/// <param name="headerValue">The existing header to be modified.</param>
	/// <exception cref="ArgumentException">The media type of the header is invalid.</exception>
	/// <exception cref="ArgumentException">Illegal parameters are present in the header.</exception>
	public MediaTypeHeaderBuilder(MediaTypeHeaderValue headerValue)
	{
		if (headerValue.MediaType != Constants.Headers.MediaType)
		{
			throw new ArgumentException(Constants.Exceptions.IllegalMediaType, nameof(headerValue));
		}
		if (headerValue.Parameters.Any(IsIllegalParameter))
		{
			throw new ArgumentException(Constants.Exceptions.IllegalHeaderParameters, nameof(headerValue));
		}

		NameValueHeaderValue? extensionsParameter = headerValue.Parameters.SingleOrDefault(IsExtensionsParameter);
		NameValueHeaderValue? profilesParameter = headerValue.Parameters.SingleOrDefault(IsProfilesParameter);

		if (extensionsParameter != null)
		{
			Extensions = ParseUrisParameter(extensionsParameter);
		}
		if (profilesParameter != null)
		{
			Profiles = ParseUrisParameter(profilesParameter);
		}
	}

	/// <summary>
	/// Creates a new builder instance.
	/// </summary>
	public MediaTypeHeaderBuilder() { }

  /// <summary>
	/// Adds an extension to be appended to the extensions parameter of the header as defined in section 5.4 of the
	/// JSON:API specification.
	/// </summary>
	/// <param name="uri">The URI of the extension to add.</param>
	/// <returns>The same builder instance for chaining expressions.</returns>
	public MediaTypeHeaderBuilder AddExtension(Uri uri)
	{
		Extensions.Add(uri);
		return this;
	}

  /// <summary>
	/// Adds a profile to be appended to the profiles parameter of the header as defined in section 5.4 of the JSON:API 
	/// specification.
	/// </summary>
	/// <param name="uri">The URI of the profile to add.</param>
	/// <returns>The same builder instance for chaining expressions.</returns>
	public MediaTypeHeaderBuilder AddProfile(Uri uri)
	{
		Profiles.Add(uri);
		return this;
	}

  /// <summary>
	/// Removes an extension from the extensions parameter of the header.
	/// </summary>
	/// <param name="uri">The URI of the extension to remove.</param>
	/// <returns>The same builder instance for chaining expressions.</returns>
	public MediaTypeHeaderBuilder RemoveExtension(Uri uri)
	{
		Extensions.Remove(uri);
		return this;
	}

  /// <summary>
	/// Removes a profile from the profiles parameter of the header.
	/// </summary>
	/// <param name="uri">The URI of the profile to remove.</param>
	/// <returns>The same builder instance for chaining expressions.</returns>
	public MediaTypeHeaderBuilder RemoveProfile(Uri uri)
	{
		Profiles.Remove(uri);
		return this;
	}

	/// <summary>
	/// Entirely removes the extensions and profiles parameters from the header along with all URIs.
	/// </summary>
	/// <returns>The same builder instance for chaining expressions.</returns>
	public MediaTypeHeaderBuilder ClearAllParameters()
	{
		Extensions.Clear();
		Profiles.Clear();
		return this;
	}

	/// <summary>
	/// Builds the header object from the current builder instance.
	/// </summary>
	/// <returns>A <see cref="MediaTypeHeaderValue"/> instance representing the header.</returns>
	public MediaTypeHeaderValue Build()
	{
		MediaTypeHeaderValue header = new(Constants.Headers.MediaType);

		if (Extensions.Count > 0)
		{
			NameValueHeaderValue extensions = BuildUrisParameter(Constants.Headers.ExtensionsParameterName, Extensions);
			header.Parameters.Add(extensions);
		}
		if (Profiles.Count > 0)
		{
			NameValueHeaderValue profiles = BuildUrisParameter(Constants.Headers.ProfilesParameterName, Profiles);
			header.Parameters.Add(profiles);
		}

		return header;
	}

	private static NameValueHeaderValue BuildUrisParameter(string name, IEnumerable<Uri> uris)
	{
		string value = uris
			.Select(uri => uri.ToString())
			.Aggregate((combined, uriString) => $"{combined} {uriString}");
		return new(name, $"\"{value}\"");
	}

	private static HashSet<Uri> ParseUrisParameter(NameValueHeaderValue parameter) 
		=> parameter.Value.ToString()
		.Trim('"')
		.Split(' ')
		.Select(uriString => new Uri(uriString))
		.ToHashSet();

	private static bool IsProfilesParameter(NameValueHeaderValue parameter) 
		=> parameter.Name.Equals(Constants.Headers.ProfilesParameterName, StringComparison.InvariantCultureIgnoreCase);
	private static bool IsExtensionsParameter(NameValueHeaderValue parameter) 
		=> parameter.Name.Equals(Constants.Headers.ExtensionsParameterName, StringComparison.InvariantCultureIgnoreCase);
	private static bool IsIllegalParameter(NameValueHeaderValue parameter) 
		=> !_legalParameterNames.Contains(parameter.Name.ToString());
}
