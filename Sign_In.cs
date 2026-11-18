using System.Text.RegularExpressions;
using System.Text.Json;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace TestProject;

[TestFixture]
public class SignInTests : PageTest
{
    private const string BaseUrl = "https://www.premieronline.com";
    private const string LoginUrlPattern = @"(?:action/dologin|login\.php)";
    private const string InvalidCredentialsMessage = "Sorry, you were not recognized, please retype your email and password again";
    private const string CredentialsFileName = "testcredentials.json";

    private ILocator Header => Page.Locator(".uk-navbar-container").First;
    private ILocator SignInLink => Header.GetByRole(
        AriaRole.Link,
        new() { Name = "Sign in", Exact = true });
    private ILocator SignInHeading => Page.GetByRole(
        AriaRole.Heading,
        new() { Name = "Sign in", Exact = true });
    private ILocator SignInForm => Page.Locator("section.register form");
    private ILocator Email => SignInForm.Locator("#login_name");
    private ILocator Password => SignInForm.Locator("#password");
    private ILocator SignInButton => SignInForm.GetByRole(
        AriaRole.Button,
        new() { Name = "Sign in", Exact = true });
    private ILocator AuthenticationError => Page.Locator(".uk-alert-danger").First;
    private ILocator ValidationErrors => Page.Locator(
        ".uk-form-danger:visible, [aria-invalid='true']:visible");

    [SetUp]
    public async Task OpenHomePageAsync()
    {
        await Page.GotoAsync(BaseUrl);
    }

    [Test]
    public async Task SignInPageOpensSuccessfully()
    {
        await OpenSignInFormAsync();

        await Expect(Page).ToHaveTitleAsync("Login");
        await Expect(Email).ToBeVisibleAsync();
        await Expect(Password).ToBeVisibleAsync();
        await Expect(SignInButton).ToBeVisibleAsync();
    }

    [Test]
    public async Task SignInRejectsEmptyCredentials()
    {
        await OpenSignInFormAsync();

        await SubmitSignInFormAsync();

        await Expect(AuthenticationError).ToBeVisibleAsync();
        await Expect(AuthenticationError).ToContainTextAsync(InvalidCredentialsMessage);
    }

    [Test]
    public async Task SignInRejectsInvalidCredentials()
    {
        await OpenSignInFormAsync();

        var invalidEmail = $"invalid-{Guid.NewGuid():N}@example.com";
        await FillSignInFormAsync(invalidEmail, "WrongPassword123");
        await SubmitSignInFormAsync();

        await Expect(AuthenticationError).ToBeVisibleAsync();
        await Expect(AuthenticationError).ToContainTextAsync(InvalidCredentialsMessage);
    }

    [Test]
    public async Task SignInSucceedsWithValidCredentials()
    {
        var credentials = await LoadTestCredentialsAsync();

        await OpenSignInFormAsync();
        await FillSignInFormAsync(credentials.Email, credentials.Password);
        await SubmitSignInFormAsync();

        await Expect(AuthenticationError).ToHaveCountAsync(0);
        await Expect(ValidationErrors).ToHaveCountAsync(0);
        await Expect(Page).Not.ToHaveURLAsync(new Regex(LoginUrlPattern));
    }

    private async Task OpenSignInFormAsync()
    {
        await Expect(SignInLink).ToBeVisibleAsync();
        await SignInLink.DispatchEventAsync("click");

        await Expect(Page).ToHaveURLAsync(new Regex(LoginUrlPattern));
        await Expect(SignInHeading).ToBeVisibleAsync();
        await Expect(Email).ToBeVisibleAsync();
        await Expect(Password).ToBeVisibleAsync();
        await Expect(SignInButton).ToBeVisibleAsync();
    }

    private async Task FillSignInFormAsync(string email, string password)
    {
        await Email.FillAsync(email);
        await Password.FillAsync(password);
    }

    private async Task SubmitSignInFormAsync()
    {
        // Sticky-header анімація робить звичайний ClickAsync нестабільним на цьому сайті.
        await SignInButton.DispatchEventAsync("click");
    }

    private static async Task<TestCredentials> LoadTestCredentialsAsync()
    {
        var credentialsPath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            CredentialsFileName);

        if (!File.Exists(credentialsPath))
        {
            Assert.Ignore(
                $"Create {CredentialsFileName} with a test email and password to run the successful sign-in test.");
        }

        var json = await File.ReadAllTextAsync(credentialsPath);
        var credentials = JsonSerializer.Deserialize<TestCredentials>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (credentials is null ||
            string.IsNullOrWhiteSpace(credentials.Email) ||
            string.IsNullOrWhiteSpace(credentials.Password))
        {
            Assert.Ignore(
                $"Add non-empty email and password values to {CredentialsFileName}.");
        }

        return credentials!;
    }

    private sealed record TestCredentials(string Email, string Password);
}
