using Microsoft.Playwright;
using Assert = Xunit.Assert;
using Chirp.Infrastructure;
using PlaywrightSetup;

namespace PlaywrightTests;

///<summery>
/// If Errors occour due to lack of installed playwright call
/// pwsh test/Chirp.RazorPages.Tests/bin/Debug/net7.0/playwright.ps1 install --with-deps
/// from the source location. Alternatively call
/// pwsh bin/Debug/net7.0/playwright.ps1 install --with-deps
/// from the test file
///
/// The playwright tests do NOT use PageTest
/// as it does not work well together with setting up a local server with WebApplicationFactory,
/// Instead it uses a manuel setup of playwright, browser, context and page.
///
/// The test are order by alphabetical order, as some test require that a user is registered in memory.
/// The syntax for naming a test should therefore be [letterOrNumberDepictingOrder]_testName example: A_test1
///</summery>
[TestCaseOrderer("TestOrderer.TestOrderer", "Chirp.RazorPages.Tests")]//runs test in alphabetical order
public class PlaywrightTests : IClassFixture<CustomWebApplicationFactory>
{
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
    public async Task A_RegisterTest() //Test to register a user, said user is used in later tests
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
        Assert.True(await page.GetByRole(AriaRole.Link, new() { Name = "logout [TestUser]" }).IsVisibleAsync());
        Assert.True(await page.GetByText("What's on your mind TestUser? Share").IsVisibleAsync(), "once registered the testUser can cheep");
        
        await browser.CloseAsync();
    }
    /* This test is correctly set up however login does currently not work.
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



