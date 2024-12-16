using Microsoft.Playwright;
using Assert = Xunit.Assert;
using PlaywrightSetup;
using Xunit.Abstractions;

namespace PlaywrightTests;

///<summery>
/// If Errors occour due to lack of installed playwright call
/// pwsh test/Chirp.UI.Tests/bin/Debug/net7.0/playwright.ps1 install --with-deps
/// from the source location. Alternatively call
/// pwsh bin/Debug/net7.0/playwright.ps1 install --with-deps
/// from the UI test file
///
/// The playwright tests do NOT use PageTest
/// as it does not work well together with setting up a local server with WebApplicationFactory,
/// Instead it uses a manuel setup of playwright, browser, context and page.
///
/// The test are order by alphabetical order, as some test require that a user is registered in memory.
/// The syntax for naming a test should therefore be [letterOrNumberDepictingOrder]_testName example: A_test1
///</summery>
[TestCaseOrderer(
    ordererTypeName: "TestOrdererSetup.TestOrderer.AlphabeticalOrderer ",
    ordererAssemblyName: "Chirp.UI.Tests"),]//runs test in alphabetical order
public class PlaywrightTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly ITestOutputHelper _output;

    private  readonly string _baseUrl;

    public PlaywrightTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _baseUrl = factory.ServerAddress;
        _output = output;
    }
    
    [Fact]
    public async Task E_SimplePublicTimeLineTest()
    {
        _output.WriteLine("Test E failed - attempted to view public timeline");
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
        _output.WriteLine("Test A failed - attempted to register a user");
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
		await page.GetByPlaceholder("John-Doe").FillAsync("TestUser");
		await page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
		await page.GetByLabel("Confirm Password").FillAsync("Password123!");
		await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        Assert.True(await page.GetByRole(AriaRole.Link, new() { Name = "logout [TestUser]" }).IsVisibleAsync());
        Assert.True(await page.GetByText("What's on your mind TestUser? Share").IsVisibleAsync(), "once registered the testUser can cheep");
        
        await browser.CloseAsync();
    }
    
    [Fact]
    public async Task B_LoginAndOutTest()
    {
        _output.WriteLine("Test B failed - Attempted to log in and out");
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
        Assert.True(await page.GetByRole(AriaRole.Link, new() { Name = "logout [TestUser]" }).IsVisibleAsync());
        Assert.True(await page.GetByText("What's on your mind TestUser? Share").IsVisibleAsync());
        await page.GetByRole(AriaRole.Link, new() { Name = "logout [TestUser]" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        Assert.True(await page.GetByText("You have successfully logged").IsVisibleAsync());
        await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        Assert.True(await page.GetByRole(AriaRole.Link, new() { Name = "login" }).IsVisibleAsync());
        
        await browser.CloseAsync();
    }


     [Fact]
    public async Task C_PostCheep()
    {
        _output.WriteLine("Test C failed - Attempted to log in and Cheep a message");
        // Start Playwright and launch the browser
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

        // Create a new browser context and page
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to the in-memory server's URL
        await page.GotoAsync(_baseUrl);
        //login to be able to cheep
        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync("TestUser@example.com");
        await page.GetByPlaceholder("password").FillAsync("Password123!");
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        //cheep a message
        await page.GetByRole(AriaRole.Textbox).FillAsync("This is a message!");
        await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        Assert.True(await page.Locator("li").Filter(new() { HasText = "TestUser This is a message" }).IsVisibleAsync());
        //Check timeline for message 
        await page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        Assert.True(await page.GetByRole(AriaRole.Heading, new() { Name = "TestUser's Timeline" }).IsVisibleAsync());
        Assert.True(await page.GetByText("Showing 1 cheeps!").IsVisibleAsync());
        Assert.True(await page.GetByText("TestUser This is a message").IsVisibleAsync());
        //check that the user also can cheep from user timeline
        Assert.True(await page.GetByText("What's on your mind TestUser? Share").IsVisibleAsync());
        
        await browser.CloseAsync();
    }
    
    [Fact]
    public async Task Y_FollowAndUnfollowAuthor()
    {
        _output.WriteLine("Test Y failed - Attempted to log in and follow an author");
        // Start Playwright and launch the browser
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

        // Create a new browser context and page
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to the in-memory server's URL
        await page.GotoAsync(_baseUrl);
        //login to be able to follow
        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync("TestUser@example.com");
        await page.GetByPlaceholder("password").FillAsync("Password123!");
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        //Follow author
        await page.GotoAsync(_baseUrl); //Public timeline
        // Verify Jacqualine Gilcoine exists in the public timeline
        Assert.True(await page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine" }).First.IsVisibleAsync());
        var li = page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine" }).First;
        await li.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync(); //Follow Author 
        //Check timeline for message from followed author
        await page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        Assert.True(await page.GetByRole(AriaRole.Heading, new() { Name = "TestUser's Timeline" }).IsVisibleAsync());
        Assert.True(await page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine" }).First.IsVisibleAsync());
        Assert.True(await page.GetByText("Unfollow").First.IsVisibleAsync());
        await page.GetByText("Unfollow").First.ClickAsync();
        Assert.False(await page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine" }).First.IsVisibleAsync());
        
        await browser.CloseAsync();
    }
}
