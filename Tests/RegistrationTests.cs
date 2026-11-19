using NUnit.Framework;
using TestProject.TestData;

namespace TestProject.Tests;

[TestFixture]
public class RegistrationTests : UiTestBase
{
    [Test]
    public async Task RegistrationFormAcceptsValidData()
    {
        await Registration.OpenAsync();
        var data = RegistrationData.Create();
        var form = Registration.Form;
        await form.FillAsync(data);

        await Expect(form.FirstName).ToHaveValueAsync(data.FirstName);
        await Expect(form.LastName).ToHaveValueAsync(data.LastName);
        await Expect(form.Email).ToHaveValueAsync(data.Email);
        var passwords = await form.PasswordsMatchAsync(data.Password, data.Password);
        Assert.That(passwords.PasswordMatches, Is.True, "Password was filled correctly.");
        Assert.That(passwords.RepeatedPasswordMatches, Is.True, "Repeated password was filled correctly.");

    }

    [Test]
    [Explicit("Requires a registration environment configured with reCAPTCHA test keys; prod rejects automated registration.")]
    public async Task RegistrationSucceedsWithValidData()
    {
        await Registration.OpenAsync();
        var data = RegistrationData.Create();
        var form = Registration.Form;
        await form.FillAsync(data);
        await form.SubmitAsync();

        await Expect(form.SuccessMessage).ToContainTextAsync($"Thank you, {data.FirstName}.");
        await Expect(form.SuccessMessage).ToContainTextAsync(
            "Please check your email for your Activation Button.");
        await Expect(form.ValidationErrors).ToHaveCountAsync(0);
    }

    [Test]
    public async Task RegistrationFormRejectsDifferentPasswords()
    {
        await Registration.OpenAsync();
        await Registration.Form.FillAsync(RegistrationData.Create(), "DifferentPass123");
        await Registration.Form.SubmitAsync();
        await Expect(Registration.Form.PasswordMismatchMessage).ToBeVisibleAsync();
    }
}
