using Microsoft.Playwright;
using TestProject.TestData;

namespace TestProject.Components;

public sealed class RegistrationFormComponent(IPage page)
{
    private ILocator Root => page.Locator("section.register");
    internal ILocator Heading => Root.GetByRole(AriaRole.Heading, new() { Name = "Create Account", Exact = true });
    internal ILocator FirstName => Root.Locator("#first_name");
    internal ILocator LastName => Root.Locator("#last_name");
    internal ILocator Email => Root.Locator("#email");
    private ILocator Password => Root.Locator("#password");
    private ILocator RepeatPassword => Root.Locator("#password_repeat");
    internal ILocator ContinueButton => Root.GetByRole(AriaRole.Button, new() { Name = "Continue", Exact = true });
    internal ILocator ValidationErrors => page.Locator(
        ".uk-alert-danger:visible, .uk-form-danger:visible, [aria-invalid='true']:visible");
    internal ILocator SuccessMessage => page.Locator(".uk-alert-success:visible");
    internal ILocator PasswordMismatchMessage => page.GetByText("Your passwords don't match.");

    public async Task ExpectReadyAsync()
    {
        await Assertions.Expect(Heading).ToBeVisibleAsync();
        foreach (var field in new[] { FirstName, LastName, Email, Password, RepeatPassword })
        {
            await Assertions.Expect(field).ToBeVisibleAsync();
            await Assertions.Expect(field).ToBeEnabledAsync();
        }
        await Assertions.Expect(ContinueButton).ToBeVisibleAsync();
    }

    internal async Task FillAsync(RegistrationData data, string? repeatedPassword = null)
    {
        await FirstName.FillAsync(data.FirstName);
        await LastName.FillAsync(data.LastName);
        await Email.FillAsync(data.Email);
        await Password.FillAsync(data.Password);
        await RepeatPassword.FillAsync(repeatedPassword ?? data.Password);
    }

    internal async Task<(bool PasswordMatches, bool RepeatedPasswordMatches)> PasswordsMatchAsync(
        string expectedPassword, string expectedRepeatedPassword) =>
        (string.Equals(await Password.InputValueAsync(), expectedPassword, StringComparison.Ordinal),
         string.Equals(await RepeatPassword.InputValueAsync(), expectedRepeatedPassword, StringComparison.Ordinal));

    public Task SubmitAsync() => ContinueButton.ClickAsync();
}
