using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Chirp.RazorPages.Tests;

public class UnitTest
{
	private HttpClient _client;

	public UnitTest()
	{
		var baseURL = "http://localhost:5273";
		_client = new();
		_client.DefaultRequestHeaders.Accept.Clear();
		_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
		_client.BaseAddress = new Uri(baseURL);
	}
	
	[Fact]
	public async void CanSeePublicTimeline()
	{
		var response = await _client.GetAsync("/");
		response.EnsureSuccessStatusCode();
		var content = await response.Content.ReadAsStringAsync();

		Assert.Contains("Chirp!", content);
		Assert.Contains("Public Timeline", content);
	}

	[Theory]
	[InlineData("Helge")]
	[InlineData("Adrian")]
	public async void CanSeePrivateTimeline(string author)
	{
		var response = await _client.GetAsync($"/{author}");
		response.EnsureSuccessStatusCode();
		var content = await response.Content.ReadAsStringAsync();

		Assert.Contains("Chirp!", content);
		Assert.Contains($"{author}'s Timeline", content);
	}
}