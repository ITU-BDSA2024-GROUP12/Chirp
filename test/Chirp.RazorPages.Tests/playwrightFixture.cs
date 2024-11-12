using Microsoft.Playwright;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading.Tasks;

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