using System.Text.RegularExpressions;
using Microsoft.Playwright;
using TestProject.Components;

namespace TestProject.Pages;

public sealed class SignInPage(IPage page)
{
    public const string LoginUrlPattern = @"(?:action/dologin|login\.php)";
    public SignInFormComponent Form { get; } = new(page);

    internal ILocator ProfileCompletionHeading => page.GetByRole(
        AriaRole.Heading, new() { Name = "Complete your profile", Exact = false });

    public async Task OpenAsync()
    {
        await new HeaderComponent(page).ClickSignInAsync();
        await Assertions.Expect(page).ToHaveURLAsync(new Regex(LoginUrlPattern));
        await Form.ExpectReadyAsync();
    }
}
