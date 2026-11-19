using Microsoft.Playwright;
using TestProject.Config;

namespace TestProject.Pages;

/// <summary>Opens the configured home screen and exposes its language-dependent state to tests.</summary>
public sealed class HomePage
{
    private readonly IPage _page;

    public HomePage(IPage page)
    {
        _page = page;
    }

    internal ILocator Html => _page.Locator("html");

    public async Task OpenHomePageAsync()
    {
        await _page.GotoAsync(TestConfig.BaseUrl.TrimEnd('/'));
    }
}
