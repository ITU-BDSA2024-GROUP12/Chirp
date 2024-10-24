using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Chirp.Infrastructure;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Chirp.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;


namespace Chirp.RazorPages.Tests;
//https://stackoverflow.com/questions/55131379/integration-testing-asp-net-core-with-net-framework-cant-find-deps-json/70490057#70490057
public class UnitTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
	private HttpClient _client;

	public UnitTest(CustomWebApplicationFactory<Program> factory)
	{
		// Use the factory to create a client
		
		_client = factory.CreateClient();
		_client.DefaultRequestHeaders.Accept.Clear();
		_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
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

