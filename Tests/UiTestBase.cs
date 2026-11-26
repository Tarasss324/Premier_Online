using Allure.Net.Commons;
using Allure.NUnit;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestProject.Components;
using TestProject.Config;
using TestProject.Pages;

namespace TestProject.Tests;

[Category("UI")]
[AllureNUnit]
public abstract class UiTestBase : PageTest
{
    protected HomePage Home { get; private set; } = null!;
    protected HeaderComponent Header { get; private set; } = null!;
    protected RegistrationPage Registration { get; private set; } = null!;
    protected SignInPage SignIn { get; private set; } = null!;

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
        Locale = "en-US"
    };

    [SetUp]
    public async Task OpenHomePageAsync()
    {
        // Validate the configured environment before sending any requests.
        _ = TestConfig.BaseUrl;
        Home = new HomePage(Page);
        Header = new HeaderComponent(Page);
        Registration = new RegistrationPage(Page);
        SignIn = new SignInPage(Page);
        await AllureApi.Step("Open the home page", () => Home.OpenHomePageAsync());
    }

    protected async Task AttachScreenshotAsync(string name)
    {
        var screenshot = await Page.ScreenshotAsync(new()
        {
            FullPage = true,
            Animations = ScreenshotAnimations.Disabled,
            Timeout = 10_000
        });

        AllureApi.AddAttachment(name, "image/png", screenshot, ".png");
    }

    [TearDown]
    public async Task AttachFailureScreenshotAsync()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed)
        {
            return;
        }

        try
        {
            await AttachScreenshotAsync("Failure screenshot");
        }
        catch (Exception exception)
        {
            TestContext.Progress.WriteLine($"Could not attach failure screenshot: {exception.Message}");
        }
    }
}
