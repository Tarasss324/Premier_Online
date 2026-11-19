using NUnit.Framework;
using TestProject.Config;

namespace TestProject.Tests;

[TestFixture]
public class HeaderTests : UiTestBase
{
    [Test]
    public async Task HeaderIsVisible()
    {
        await Expect(Header.Root).ToBeVisibleAsync();
        await Expect(Header.Logo).ToHaveAttributeAsync("href", TestConfig.BaseUrl);
        await Expect(Header.LogoImage).ToHaveAttributeAsync("alt", "Premiere Online");
    }

    [Test]
    public async Task MobileMenuOpens()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await Header.OpenMobileMenuAsync();
        await Expect(Header.MobileMenu).ToBeVisibleAsync();
        await Expect(Header.MobileMenuNavigation).ToBeVisibleAsync();
        await Expect(Header.MobileMenuEventsLink).ToBeVisibleAsync();
    }

    [Test]
    public async Task HeaderLinksAreVisible()
    {
        foreach (var name in new[] { "Events", "Ratings", "Help", "عربى" })
        {
            await Expect(Header.HeaderLink(name)).ToBeVisibleAsync();
        }
    }

    [TestCase("Events", "/calendar")]
    [TestCase("Ratings", "/event_ratings.php")]
    [TestCase("Help", "/#help")]
    public async Task HeaderLinkNavigatesCorrectly(string linkName, string expectedPath)
    {
        await Header.ClickHeaderLinkAsync(linkName);
        await Expect(Page).ToHaveURLAsync($"{TestConfig.BaseUrl}{expectedPath}");
    }
}
