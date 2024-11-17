using Microsoft.Playwright;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using System.Diagnostics;
using NUnit.Framework;

namespace PlaywrightTests;


//setting up local host for tests to run on
[SetUpFixture]
public class TestSetup
{
    private Process _webServerProcess;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // Start the development server
        _webServerProcess = new Process();
		_webServerProcess.StartInfo.FileName = "dotnet";
        _webServerProcess.StartInfo.Arguments = "run --urls \"http://localhost:5002\""; //designated port
        _webServerProcess.StartInfo.WorkingDirectory = @"..\..\..\..\..\src\Chirp.Web"; //Runs from the .net in bin file
        _webServerProcess.StartInfo.CreateNoWindow = true;
        _webServerProcess.StartInfo.UseShellExecute = false;
        _webServerProcess.Start();
            
		//delay to let it spin up
       Task.Delay(5000).Wait();
    }
	
	//ends localhost once tests are over
    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        // Stop the server
        if (!_webServerProcess.HasExited)
        {
            _webServerProcess.Kill();
        }
    }
}


[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : PageTest
{

	//tests go here:

    [Test] //tests for basic information that should be visible on public timeline. (un-loggedIn)
    public async Task publicTimeLineTest()
    {
		await Page.GotoAsync("http://localhost:5002");
		await Page.GetByRole(AriaRole.Heading, new() { Name = "Icon1Chirp!" }).ClickAsync();
		await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
        await Expect(Page.Locator("div").Filter(new() { HasText = "Public Timeline Showing 32" }).Nth(1)).ToBeVisibleAsync();
        await Expect(Page.GetByText("Showing 32 cheeps!")).ToBeVisibleAsync();
    }
    
     [Test] //tests for basic information that should be visible on public timeline. (un-loggedIn)
        public async Task registerTest()
        {
    		await Page.GotoAsync("http://localhost:5002");
    		await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "register" })).ToBeVisibleAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
      		await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
            await Page.GetByPlaceholder("John Doe").ClickAsync();
            await Page.GetByPlaceholder("John Doe").FillAsync("testUser1");
            await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
            await Page.GetByLabel("Confirm Password").ClickAsync();
            await Page.GetByLabel("Confirm Password").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Register confirmation" })).ToBeVisibleAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
            await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "logout [testUser1]" })).ToBeVisibleAsync();
            await Expect(Page.GetByText("What's on your mind testUser1? Share")).ToBeVisibleAsync();
        }
    
    
    

}


