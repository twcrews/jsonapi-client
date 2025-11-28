using Microsoft.Net.Http.Headers;
using Crews.Web.JsonApiClient.Utility;

namespace Crews.Web.JsonApiClient.Tests.Utility;

public class MediaTypeHeaderBuilderTests
{
	private const string MediaType = "application/vnd.api+json";

	[Theory(DisplayName = "Constructor correctly parses header value")]
	[InlineData(
		new string[] { },
		new string[] { })]
	[InlineData(
		new string[] { "http://ext1.com/", "http://ext2.com/", "http://ext3.com/" },
		new string[] { "http://profile1.com/", "http://profile2.com/" })]
    public void ConstructorParsesHeader(string[] extensionStrings, string[] profileStrings)
	{
		HashSet<Uri> extensions = extensionStrings
			.Select(str => new Uri(str))
			.ToHashSet();
		HashSet<Uri> profiles = profileStrings
			.Select(str => new Uri(str))
			.ToHashSet();

		MediaTypeHeaderValue header = new(MediaType);

		if (extensions.Count > 0)
		{
			string combinedExtensions = extensionStrings
				.Aggregate((combined, extension) => $"{combined} {extension}");
			header.Parameters.Add(new("ext", $"\"{combinedExtensions}\""));
		}
		if (profiles.Count > 0)
		{
			string combinedProfiles = profileStrings
				.Aggregate((combined, profile) => $"{combined} {profile}");
			header.Parameters.Add(new("profile", $"\"{combinedProfiles}\""));
		}

		MediaTypeHeaderBuilder builder = new(header);

		Assert.Equal(extensions, builder.Extensions);
		Assert.Equal(profiles, builder.Profiles);
	}

	[Fact(DisplayName = "Constructor throws exception when header has invalid media type")]
	public void ConstructorThrowsOnIllegalMediaType() 
		=> Assert.Throws<ArgumentException>(() => new MediaTypeHeaderBuilder(new("image/png")));

	[Fact(DisplayName = "Constructor throws exception when header contains invalid parameters")]
	public void ConstructorThrowsOnIllegalParameters()
	{
		MediaTypeHeaderValue header = new(MediaType);
		header.Parameters.Add(new("myParameter", "abc123"));
		Assert.Throws<ArgumentException>(() => new MediaTypeHeaderBuilder(header));
	}

	[Fact(DisplayName = "AddExtension correctly modifies Extensions property")]
	public void AddExtensionAddsExtension()
	{
		Uri uri = new("https://ext.com/");
		MediaTypeHeaderBuilder builder = new();
		Assert.DoesNotContain(uri, builder.Extensions);

		builder.AddExtension(uri);
		Assert.Contains(uri, builder.Extensions);
	}

	[Fact(DisplayName = "AddProfile correctly modifies Profiles property")]
	public void AddProfileAddsProfile()
	{
		Uri uri = new("https://profile.com/");
		MediaTypeHeaderBuilder builder = new();
		Assert.DoesNotContain(uri, builder.Profiles);
		builder.AddProfile(uri);
		Assert.Contains(uri, builder.Profiles);
	}

	[Fact(DisplayName = "RemoveExtension correctly modifies Extensions property")]
	public void RemoveExtensionRemovesExtension()
	{
		Uri uri = new("https://ext.com/");
		MediaTypeHeaderBuilder builder = new()
		{
			Extensions = [uri]
		};
		Assert.Contains(uri, builder.Extensions);

		builder.RemoveExtension(uri);
		Assert.DoesNotContain(uri, builder.Extensions);
	}

	[Fact(DisplayName = "RemoveProfile correctly modifies Profiles property")]
	public void RemoveProfileRemovesProfile()
	{
		Uri uri = new("https://profile.com/");
		MediaTypeHeaderBuilder builder = new()
		{
			Profiles = [uri]
		};
		Assert.Contains(uri, builder.Profiles);

		builder.RemoveProfile(uri);
		Assert.DoesNotContain(uri, builder.Profiles);
	}

	[Fact(DisplayName = "ClearAllParameters correctly modifies Extensions, Profiles, and CustomParameters properties")]
	public void ClearAllParametersClearsAllParameters()
	{
		Uri extension = new("https://ext.com/");
		Uri profile = new("https://profile.com/");
		NameValueHeaderValue customParameter = new("customParam", "abc123");

		MediaTypeHeaderBuilder builder = new()
		{
			Extensions = [extension],
			Profiles = [profile]
		};
		Assert.NotEmpty(builder.Extensions);
		Assert.NotEmpty(builder.Profiles);

		builder.ClearAllParameters();
		Assert.Empty(builder.Extensions);
		Assert.Empty(builder.Profiles);
	}

	[Theory(DisplayName = "Build method correctly builds header object")]
	[InlineData(
		new string[] { },
		new string[] { })]
	[InlineData(
		new string[] { "http://ext1.com/", "http://ext2.com/", "http://ext3.com/" },
		new string[] { "http://profile1.com/", "http://profile2.com/" })]
	public void BuildBuildsHeader(
		string[] extensionStrings,
		string[] profileStrings)
	{
		HashSet<Uri> extensions = extensionStrings
			.Select(str => new Uri(str))
			.ToHashSet();
		HashSet<Uri> profiles = profileStrings
			.Select(str => new Uri(str))
			.ToHashSet();
			
		MediaTypeHeaderBuilder builder = new()
		{
			Extensions = extensions,
			Profiles = profiles,
		};

		MediaTypeHeaderValue expected = new(MediaType);
		if (extensions.Count > 0)
		{
			string combinedExtensions = extensionStrings
				.Aggregate((combined, extension) => $"{combined} {extension}");
			expected.Parameters.Add(new("ext", $"\"{combinedExtensions}\""));
		}
		if (profiles.Count > 0)
		{
			string combinedProfiles = profileStrings
				.Aggregate((combined, profile) => $"{combined} {profile}");
			expected.Parameters.Add(new("profile", $"\"{combinedProfiles}\""));
		}

		Assert.Equal(expected, builder.Build());
	}
}
