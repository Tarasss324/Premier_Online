using Allure.Net.Commons;
using NUnit.Framework;
using TestProject.TestData;

namespace TestProject.Tests;

[TestFixture]
public class RegistrationTests : UiTestBase
{
    [Test]
    public async Task RegistrationFormAcceptsValidData()
    {
        await OpenRegistrationFormAsync();
        var data = RegistrationData.Create();
        var form = Registration.Form;
        await FillRegistrationFormAsync(data);

        await AllureApi.Step("Verify registration fields contain the entered data", async () =>
        {
            await Expect(form.FirstName).ToHaveValueAsync(data.FirstName);
            await Expect(form.LastName).ToHaveValueAsync(data.LastName);
            await Expect(form.Email).ToHaveValueAsync(data.Email);
            var passwords = await form.PasswordsMatchAsync(data.Password, data.Password);
            Assert.That(passwords.PasswordMatches, Is.True, "Password was filled correctly.");
            Assert.That(passwords.RepeatedPasswordMatches, Is.True, "Repeated password was filled correctly.");
            await AttachScreenshotAsync("Registration field values verified");
        });
    }

    [Test]
    [Explicit("Requires a registration environment configured with reCAPTCHA test keys; prod rejects automated registration.")]
    public async Task RegistrationSucceedsWithValidData()
    {
        await OpenRegistrationFormAsync();
        var data = RegistrationData.Create();
        var form = Registration.Form;
        await FillRegistrationFormAsync(data);
        await AllureApi.Step("Submit the registration form", () => form.SubmitAsync());

        await AllureApi.Step("Verify successful registration and the email activation instruction", async () =>
        {
            await Expect(form.SuccessMessage).ToContainTextAsync($"Thank you, {data.FirstName}.");
            await Expect(form.SuccessMessage).ToContainTextAsync(
                "Please check your email for your Activation Button.");
            await Expect(form.ValidationErrors).ToHaveCountAsync(0);
            await AttachScreenshotAsync("Registration success message");
        });
    }

    [Test]
    public async Task RegistrationFormRejectsDifferentPasswords()
    {
        await OpenRegistrationFormAsync();
        await FillRegistrationFormAsync(RegistrationData.Create(), "DifferentPass123");
        await AllureApi.Step("Submit the registration form with different passwords",
            () => Registration.Form.SubmitAsync());

        await AllureApi.Step("Verify the password mismatch validation message", async () =>
        {
            await Expect(Registration.Form.PasswordMismatchMessage).ToBeVisibleAsync();
            await AttachScreenshotAsync("Registration password mismatch validation");
        });
    }

    private Task OpenRegistrationFormAsync() =>
        AllureApi.Step("Open the registration form and verify it is ready", async () =>
        {
            await Registration.OpenAsync();
            await AttachScreenshotAsync("Registration form ready");
        });

    private Task FillRegistrationFormAsync(RegistrationData data, string? repeatedPassword = null) =>
        AllureApi.Step(repeatedPassword is null
            ? "Fill the registration form with valid data"
            : "Fill the registration form with different passwords", async () =>
        {
            await Registration.Form.FillAsync(data, repeatedPassword);
            await AttachScreenshotAsync("Registration form after data entry");
        });
}
