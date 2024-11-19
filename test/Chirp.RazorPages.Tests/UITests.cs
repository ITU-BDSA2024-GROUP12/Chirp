using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Xunit.Microsoft.DependencyInjection.TestsOrder;
using Microsoft.Playwright;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Xunit.Abstractions;
using Xunit.Sdk;
using Assert = Xunit.Assert;

namespace PlaywrightTests;

// the webAllplicationFactory is taken from https://github.com/donbavand/playwright-webapplicationfactory/blob/main/Playwright.App.Tests/BlazorUiTests.cs
//with Apache License 2.0. The software has been modified with the addition of the ConfigureServices.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    
    private IHost? _host;

    public string ServerAddress
    {
        get
        {
            EnsureServer();
            return ClientOptions.BaseAddress.ToString();
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CheepDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Use SQLite in-memory database
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open(); // Keep the connection open for the duration of the test

            services.AddDbContext<CheepDbContext>(options =>
            {
                options.UseSqlite(connection);
            });

            // Build the service provider and create the schema
            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CheepDbContext>();

                context.Database.EnsureDeleted(); // Optional: Clear any previous state
                context.Database.EnsureCreated(); // Creates the database schema
                DbInitializer.SeedDatabase(context);                // Seed test data
            }
        });
        
        // Create the host for TestServer now before we
        // modify the builder to use Kestrel instead.
        var testHost = builder.Build();

        // Modify the host builder to use Kestrel instead
        // of TestServer so we can listen on a real address.
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());

        // Create and start the Kestrel server before the test server,
        // otherwise due to the way the deferred host builder works
        // for minimal hosting, the server will not get "initialized
        // enough" for the address it is listening on to be available.
        // See https://github.com/dotnet/aspnetcore/issues/33846.
        _host = builder.Build();
        _host.Start();

        // Extract the selected dynamic port out of the Kestrel server
        // and assign it onto the client options for convenience so it
        // "just works" as otherwise it'll be the default http://localhost
        // URL, which won't route to the Kestrel-hosted HTTP server.
         var server = _host.Services.GetRequiredService<IServer>();
         var addresses = server.Features.Get<IServerAddressesFeature>();

        ClientOptions.BaseAddress = addresses!.Addresses
            .Select(x => new Uri(x))
            .Last();
        
        // Return the host that uses TestServer, rather than the real one.
        // Otherwise the internals will complain about the host's server
        // not being an instance of the concrete type TestServer.
        // See https://github.com/dotnet/aspnetcore/pull/34702.
        testHost.Start();
        return testHost;
    }
    
    protected override void Dispose(bool disposing)
    {
        _host?.Dispose();
    }

    private void EnsureServer()
    {
        if (_host is null)
        {
            // This forces WebApplicationFactory to bootstrap the server
            using var _ = CreateDefaultClient();
        }
    }
}

public class TestOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        // Example: Order tests alphabetically by method name
        return testCases.OrderBy(tc => tc.TestMethod.Method.Name);
    }
}


[TestCaseOrderer("PlaywrightTests.TestOrderer", "Chirp.RazorPages.Tests")]//runs test in alphabetical order
public class PlaywrightTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private  readonly string _baseUrl;

    public PlaywrightTests(CustomWebApplicationFactory factory)
    {
        _baseUrl = factory.ServerAddress;
    }
    
    [Fact]
    public async Task C_SimplePublicTimeLineTest()
    {
        // Start Playwright and launch the browser
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

        // Create a new browser context and page
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to the in-memory server's URL
        await page.GotoAsync(_baseUrl);
        await page.GetByRole(AriaRole.Heading, new() { Name = "Icon1Chirp!" }).ClickAsync();
        Assert.True(await page.Locator("role=heading[name='Public Timeline']").IsVisibleAsync());
		
        await browser.CloseAsync();
    }
    
    
	 [Fact]
    public async Task A_RegisterTest() //first register a user in temp db
    {
        // Start Playwright and launch the browser
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

        // Create a new browser context and page
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to the in-memory server's URL
        await page.GotoAsync(_baseUrl);
        Assert.True(await page.GetByRole(AriaRole.Link, new() { Name = "register" }).IsVisibleAsync());
		await page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
		await page.GetByPlaceholder("name@example.com").FillAsync("TestUser@example.com");
		await page.GetByPlaceholder("John Doe").FillAsync("TestUser");
		await page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
		await page.GetByLabel("Confirm Password").FillAsync("Password123!");
		await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        Assert.True(await page.GetByRole(AriaRole.Link, new() { Name = "logout [TestUser@example.com]" }).IsVisibleAsync());
        Assert.True(await page.GetByText("What's on your mind TestUser@example.com? Share").IsVisibleAsync(), "once registered the testUser can cheep");
        
        await browser.CloseAsync();
    }
    /*
    [Fact]
    public async Task B_LoginAndOutTest()
    {
        // Start Playwright and launch the browser
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

        // Create a new browser context and page
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to the in-memory server's URL
        await page.GotoAsync(_baseUrl);
        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync("TestUser@example.com");
        await page.GetByPlaceholder("password").FillAsync("Password123!");
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
       // Assert.True(await page.GetByRole(AriaRole.Link, new() { Name = "logout [TestUser@example.com]" }).IsVisibleAsync());
        Assert.True(await page.GetByText("What's on your mind TestUser@example.com? Share").IsVisibleAsync());
        await page.GetByRole(AriaRole.Link, new() { Name = "logout [TestUser@example.com]" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        Assert.True(await page.GetByText("You have successfully logged").IsVisibleAsync());
        await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        Assert.True(await page.GetByRole(AriaRole.Link, new() { Name = "login" }).IsVisibleAsync());
    }*/
}



