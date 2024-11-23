using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
namespace Chirp.RazorPages.Tests
{
    /*public class EndToEnd : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Program> _factory;
        public EndToEnd(CustomWebApplicationFactory<Program> factory)
        {
            // Use the factory to create a client
            //_client = factory.CreateClient();
            //_client.DefaultRequestHeaders.Accept.Clear();
            //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            _factory = factory;
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
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
    }*/
}
