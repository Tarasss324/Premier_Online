using System.Text.RegularExpressions;
using Microsoft.Playwright;
using TestProject.Components;

namespace TestProject.Pages;

public sealed class RegistrationPage(IPage page)
{
    public RegistrationFormComponent Form { get; } = new(page);

    public async Task OpenAsync()
    {
        await new HeaderComponent(page).ClickCreateAccountAsync();
        await Assertions.Expect(page).ToHaveURLAsync(new Regex("register"));
        await Form.ExpectReadyAsync();
    }
}
