using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : PageTest
{
    [Test]
    public async Task GetStartedLink()
    {
		await Page.GotoAsync("http://127.0.0.1:3000");
		await Page.GetByRole(AriaRole.Heading, new() { Name = "Icon1Chirp!" }).ClickAsync();
		await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
    } 
}