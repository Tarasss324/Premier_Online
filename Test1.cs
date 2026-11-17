using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace TestProject;

[TestFixture]
public class Test : PageTest
{
    private const string BaseUrl = "https://www.premieronline.com";
    private const string HomePageTitle =
        "Premier Online - leading provider of online event registration for sports events";
    private const string TestPassword = "TestPass123";

    private ILocator Header => Page.Locator(".uk-navbar-container").First;
    private ILocator Logo => Header.Locator("a.uk-logo");
    private ILocator LogoImage => Logo.GetByRole(AriaRole.Img);
    private ILocator MenuButton => Page.Locator("button:has(i.fa-bars)");
    private ILocator HeaderNavigation => Page.Locator("nav .uk-navbar-left > ul.uk-navbar-nav");
    private ILocator CreateAccountLink => Header.GetByRole(
        AriaRole.Link,
        new() { Name = "Create Account", Exact = true });
    private ILocator RegistrationHeading => Page.GetByRole(
        AriaRole.Heading,
        new() { Name = "Create Account", Exact = true });
    private ILocator FirstName => Page.Locator("#first_name");
    private ILocator LastName => Page.Locator("#last_name");
    private ILocator Email => Page.Locator("#email");
    private ILocator Password => Page.Locator("#password");
    private ILocator RepeatPassword => Page.Locator("#password_repeat");
    private ILocator ContinueButton => Page.GetByRole(AriaRole.Button, new() { Name = "Continue" });

    private ILocator HeaderLink(string name) =>
        HeaderNavigation.GetByRole(
            AriaRole.Link,
            new() { Name = name, Exact = true });

    [SetUp]
    public async Task OpenHomePageAsync()
    {
        // Кожен тест починається з незалежного стану головної сторінки.
        await Page.GotoAsync(BaseUrl);
    }

    [Test]
    public async Task HomePageOpensSuccessfully()
    {
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/");
        await Expect(Page).ToHaveTitleAsync(HomePageTitle);
    }

    [Test]
    public async Task HeaderIsVisible()
    {
        await Expect(Header).ToBeVisibleAsync();
        await Expect(Logo).ToHaveAttributeAsync("href", BaseUrl);
        await Expect(LogoImage).ToHaveAttributeAsync("alt", "Premiere Online");
    }

    [Test]
    public async Task MobileMenuOpens()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await MenuButton.ClickAsync();

        // Перевіряємо результат дії, а не лише те, що клік не завершився помилкою.
        await Expect(HeaderNavigation).ToBeVisibleAsync();
    }

    [Test]
    public async Task HeaderLinksAreVisible()
    {
        await Expect(HeaderLink("Events")).ToBeVisibleAsync();
        await Expect(HeaderLink("Ratings")).ToBeVisibleAsync();
        await Expect(HeaderLink("Help")).ToBeVisibleAsync();
        await Expect(HeaderLink("عربى")).ToBeVisibleAsync();
    }

    [TestCase("Events", "https://www.premieronline.com/calendar")]
    [TestCase("Ratings", "https://www.premieronline.com/event_ratings.php")]
    [TestCase("Help", "https://www.premieronline.com/#help")]
    public async Task HeaderLinkNavigatesCorrectly(string linkName, string expectedUrl)
    {
        await HeaderLink(linkName).ClickAsync();

        await Expect(Page).ToHaveURLAsync(expectedUrl);
    }

    [Test]
    public async Task ArabicLanguageIsApplied()
    {
        await HeaderLink("عربى").ClickAsync();

        await Expect(Page.Locator("html")).ToHaveAttributeAsync("dir", "rtl");
        await Expect(Page.Locator("#search")).ToHaveAttributeAsync(
            "placeholder",
            "البحث عن طريق اسم الحدث أو المنظم");
        await Expect(HeaderLink("الفعاليات")).ToBeVisibleAsync();
        await Expect(HeaderLink("مساعدة")).ToBeVisibleAsync();
        await Expect(HeaderLink("English")).ToBeVisibleAsync();
    }

    [Test]
    public async Task RegistrationFormCanBeFilled()
    {
        await OpenRegistrationFormAsync();

        var uniqueEmail = $"test-{Guid.NewGuid():N}@example.com";

        await FillRegistrationFormAsync(uniqueEmail, TestPassword, TestPassword);

        await Expect(FirstName).ToHaveValueAsync("Test");
        await Expect(LastName).ToHaveValueAsync("User");
        await Expect(Email).ToHaveValueAsync(uniqueEmail);
        await Expect(Password).ToHaveValueAsync(TestPassword);
        await Expect(RepeatPassword).ToHaveValueAsync(TestPassword);
    }

    [Test]
    public async Task RegistrationFormRejectsDifferentPasswords()
    {
        await OpenRegistrationFormAsync();

        var uniqueEmail = $"test-{Guid.NewGuid():N}@example.com";
        await FillRegistrationFormAsync(uniqueEmail, TestPassword, "DifferentPass123");

        await ContinueButton.ClickAsync();

        await Expect(Page.GetByText("Your passwords don't match."))
            .ToBeVisibleAsync();
    }

    private async Task OpenRegistrationFormAsync()
    {
        await CreateAccountLink.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("register"));
        await Expect(RegistrationHeading).ToBeVisibleAsync();

        foreach (var field in new[] { FirstName, LastName, Email, Password, RepeatPassword })
        {
            await Expect(field).ToBeVisibleAsync();
            await Expect(field).ToBeEnabledAsync();
        }

        await Expect(ContinueButton).ToBeVisibleAsync();
    }

    private async Task FillRegistrationFormAsync(
        string email,
        string password,
        string repeatedPassword)
    {
        await FirstName.FillAsync("Test");
        await LastName.FillAsync("User");
        await Email.FillAsync(email);
        await Password.FillAsync(password);
        await RepeatPassword.FillAsync(repeatedPassword);
    }
}

