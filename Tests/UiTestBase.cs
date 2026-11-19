using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using TestProject.Components;
using TestProject.Config;
using TestProject.Pages;

namespace TestProject.Tests;

[Category("UI")]
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
        await Home.OpenHomePageAsync();
    }
}
