using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace TestProject;

[TestFixture]
public class Test : PageTest
{
    private ILocator Header => Page.Locator(".uk-navbar-container").First;     
    private ILocator Logo => Header.Locator("a.uk-logo");
    private ILocator LogoImage => Logo.GetByRole(AriaRole.Img);
    private ILocator MenuButton => Page.Locator("button:has(i.fa-bars)");
   private ILocator HeaderNavigation => Page.Locator("nav .uk-navbar-left > ul.uk-navbar-nav");
   private ILocator HeaderLink(string name) =>
    HeaderNavigation.GetByRole(
        AriaRole.Link,
        new() { Name = name, Exact = true });

    [Test]
    public async Task OpenPage()
    {
        await Page.GotoAsync("https://www.premieronline.com");
        await Expect(Page).ToHaveURLAsync("https://www.premieronline.com/");
        await Expect(Page).ToHaveTitleAsync("Premier Online - leading provider of online event registration for sports events");
    }

    [Test]
    public async Task HeaderVisible()
    {
        await Page.GotoAsync("https://www.premieronline.com");
        await Expect(Header).ToBeVisibleAsync();
        await Expect(Logo).ToHaveAttributeAsync("href", "https://www.premieronline.com");
        await Expect(LogoImage).ToHaveAttributeAsync("alt", "Premiere Online");
    }

    [Test]
    public async Task OpenMenu()
    {
        await Page.GotoAsync("https://www.premieronline.com");
        await Page.SetViewportSizeAsync(390, 844);
        await MenuButton.ClickAsync();
    }

    [Test]
    public async Task HeaderLinksAreVisible()
    {
    await Page.GotoAsync("https://www.premieronline.com");

    await Expect(HeaderLink("Events")).ToBeVisibleAsync();
    await Expect(HeaderLink("Ratings")).ToBeVisibleAsync();
    await Expect(HeaderLink("Help")).ToBeVisibleAsync();
    await Expect(HeaderLink("عربى")).ToBeVisibleAsync();
    }

    [TestCase("Events", "https://www.premieronline.com/calendar")]
    [TestCase("Ratings", "https://www.premieronline.com/event_ratings.php")]
    [TestCase("Help", "https://www.premieronline.com/#help")]
    public async Task HeaderLinkNavigatesCorrectly(
    string linkName,
    string expectedUrl)
    {
    await Page.GotoAsync("https://www.premieronline.com");

    await HeaderLink(linkName).ClickAsync();

    await Expect(Page).ToHaveURLAsync(expectedUrl);
    }

    [Test]
    public async Task ArabicLanguageIsApplied()
    {
    await Page.GotoAsync("https://www.premieronline.com");

    await HeaderLink("عربى").ClickAsync();

    await Expect(Page.Locator("html"))
        .ToHaveAttributeAsync("dir", "rtl");

    await Expect(Page.Locator("#search"))
        .ToHaveAttributeAsync(
            "placeholder",
            "البحث عن طريق اسم الحدث أو المنظم");

    await Expect(HeaderLink("الفعاليات"))
        .ToBeVisibleAsync();

    await Expect(HeaderLink("مساعدة"))
        .ToBeVisibleAsync();

    await Expect(HeaderLink("English"))
        .ToBeVisibleAsync();
    }


    
}

