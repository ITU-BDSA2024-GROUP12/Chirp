using Microsoft.Playwright;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using System.Diagnostics;
using NUnit.Framework;

namespace PlaywrightTests;

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
        _webServerProcess.StartInfo.Arguments = "run --urls \"http://localhost:5002\"";
        _webServerProcess.StartInfo.WorkingDirectory = @"..\..\..\..\..\src\Chirp.Web";
        _webServerProcess.StartInfo.CreateNoWindow = true;
        _webServerProcess.StartInfo.UseShellExecute = false;
        _webServerProcess.Start();
            
		//delay to let it spin up
       Task.Delay(6000).Wait();
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
		await Page.GotoAsync("http://localhost:5002");
		await Page.GetByRole(AriaRole.Heading, new() { Name = "Icon1Chirp!" }).ClickAsync();
		await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
    } 

    }


