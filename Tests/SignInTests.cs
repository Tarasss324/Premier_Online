using System.Text.RegularExpressions;
using NUnit.Framework;
using TestProject.Components;
using TestProject.Pages;
using TestProject.TestData;

namespace TestProject.Tests;

[TestFixture]
public class SignInTests : UiTestBase
{
    [Test]
    public async Task SignInPageOpensSuccessfully()
    {
        await SignIn.OpenAsync();
        await Expect(Page).ToHaveTitleAsync("Login");
    }

    [Test]
    public async Task SignInRejectsEmptyCredentials()
    {
        await SignIn.OpenAsync();
        await SignIn.Form.SubmitAsync();
        await Expect(SignIn.Form.AuthenticationError).ToContainTextAsync(SignInFormComponent.InvalidCredentialsMessage);
    }

    [Test]
    public async Task SignInRejectsInvalidCredentials()
    {
        await SignIn.OpenAsync();
        await SignIn.Form.FillAsync($"invalid-{Guid.NewGuid():N}@example.com", "WrongPassword123");
        await SignIn.Form.SubmitAsync();
        await Expect(SignIn.Form.AuthenticationError).ToContainTextAsync(SignInFormComponent.InvalidCredentialsMessage);
    }

    [Test]
    public async Task SignInSucceedsWithValidCredentials()
    {
        var credentials = await TestCredentials.LoadAsync();
        await SignIn.OpenAsync();
        await SignIn.Form.FillAsync(credentials.Email, credentials.Password);
        await SignIn.Form.SubmitAsync();

        await Expect(Page).Not.ToHaveURLAsync(new Regex(SignInPage.LoginUrlPattern));
        await Expect(SignIn.Form.AuthenticationError).ToHaveCountAsync(0);
        await Expect(SignIn.Form.ValidationErrors).ToHaveCountAsync(0);
        // A successful login may first require completing the test account's profile.
        await Expect(Header.SignOutLink.Or(SignIn.ProfileCompletionHeading).First).ToBeVisibleAsync();
    }
}
