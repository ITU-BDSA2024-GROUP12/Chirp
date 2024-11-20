using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chirp.RazorPages.Tests;
//https://stackoverflow.com/questions/55131379/integration-testing-asp-net-core-with-net-framework-cant-find-deps-json/70490057#70490057
public class UnitTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
	private HttpClient _client;
	private WebApplicationFactory<Program> _factory;

	public UnitTest(CustomWebApplicationFactory<Program> factory)
	{
		// Use the factory to create a client
		_factory = factory;
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
	
	[Fact]
	public async void TestLoginAndSendCheep()
    {
                // Arrange
                var client = _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication(defaultScheme: "TestScheme")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "TestScheme", options => { });
                    });
                })
                    .CreateClient(new WebApplicationFactoryClientOptions
                    {
                        AllowAutoRedirect = false,
                    });
    
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(scheme: "TestScheme");
    
                //Act
                var response = await client.GetAsync("/Public");
				var response2 = await client.PostAsync("/Public",new StringContent("testCheep"));
				
                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
				//Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

    }
        }

class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
		: base(options, logger, encoder, clock)
	{
	}
    
	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
		var identity = new ClaimsIdentity(claims, "Test");
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, "TestScheme");
    
		var result = AuthenticateResult.Success(ticket);
    
		return Task.FromResult(result);
	}
}

