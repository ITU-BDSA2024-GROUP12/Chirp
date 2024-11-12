using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Diagnostics;
using NUnit.Framework;

namespace PlaywrightTests;

[SetUpFixture]
public class TestSetup
{
    private Process _webServerProcess;
    public static string BaseUrl = "http://localhost:3000";

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // Start the development server
        _webServerProcess = new Process();
        _webServerProcess.StartInfo.FileName = "cmd.exe";
        _webServerProcess.StartInfo.Arguments = "dotnet run";
        _webServerProcess.StartInfo.WorkingDirectory = @"..\..\src\Chirp.Web";
        _webServerProcess.StartInfo.CreateNoWindow = true;
        _webServerProcess.StartInfo.UseShellExecute = false;
        _webServerProcess.Start();
            
        // Optional: Add a wait here if the server takes time to spin up
        Task.Delay(3000).Wait();
    }

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


    [Test]
    public async Task GetStartedLink()
    {
		await Page.GotoAsync("/");
		await Page.GetByRole(AriaRole.Heading, new() { Name = "Icon1Chirp!" }).ClickAsync();
		await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
    } 

}