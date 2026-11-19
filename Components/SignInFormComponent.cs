using Microsoft.Playwright;

namespace TestProject.Components;

public sealed class SignInFormComponent(IPage page)
{
    public const string InvalidCredentialsMessage =
        "Sorry, you were not recognized, please retype your email and password again";

    private ILocator Root => page.Locator("section.register");
    internal ILocator Heading => Root.GetByRole(AriaRole.Heading, new() { Name = "Sign in", Exact = true });
    private ILocator Form => Root.Locator("form");
    internal ILocator Email => Form.Locator("#login_name");
    private ILocator Password => Form.Locator("#password");
    internal ILocator SignInButton => Form.GetByRole(AriaRole.Button, new() { Name = "Sign in", Exact = true });
    internal ILocator AuthenticationError => page.Locator(".uk-alert-danger:visible");
    internal ILocator ValidationErrors => page.Locator(".uk-form-danger:visible, [aria-invalid='true']:visible");

    public async Task ExpectReadyAsync()
    {
        await Assertions.Expect(Heading).ToBeVisibleAsync();
        await Assertions.Expect(Email).ToBeVisibleAsync();
        await Assertions.Expect(Password).ToBeVisibleAsync();
        await Assertions.Expect(SignInButton).ToBeVisibleAsync();
    }

    public async Task FillAsync(string email, string password)
    {
        await Email.FillAsync(email);
        await Password.FillAsync(password);
    }

    public Task SubmitAsync() => SignInButton.ClickAsync();
}
