using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;


[TestFixture]
public class EndToEndTests : PageTest
{
  private Process _serverProcess;

  [SetUp]
  public async Task Init()
  {
    _serverProcess = await MyEndToEndUtil.StartServer(); // Custom utility class - not part of Playwright
  }

  [TearDown]
  public async Task Cleanup()
  {
      _serverProcess.Kill();
      _serverProcess.Dispose();
  }
//^taken from https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_09 

  // Test cases ...
}