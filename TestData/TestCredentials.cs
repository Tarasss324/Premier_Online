using System.Text.Json;
using NUnit.Framework;
using TestProject.Config;

namespace TestProject.TestData;

internal sealed record TestCredentials(string Email, string Password)
{
    public static async Task<TestCredentials> LoadAsync()
    {
        var email = TestConfig.GetOptional("TEST_USER_EMAIL");
        var password = TestConfig.GetOptional("TEST_USER_PASSWORD");
        if (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(password))
        {
            return RequireComplete(email, password);
        }

        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "testcredentials.json");
        if (!File.Exists(path))
        {
            Assert.Ignore("Set TEST_USER_EMAIL and TEST_USER_PASSWORD or create testcredentials.json to run the successful sign-in test.");
        }

        TestCredentials? credentials;
        try
        {
            credentials = JsonSerializer.Deserialize<TestCredentials>(
                await File.ReadAllTextAsync(path),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("testcredentials.json must contain a JSON object with email and password fields.");
        }

        return RequireComplete(credentials?.Email, credentials?.Password);
    }

    private static TestCredentials RequireComplete(string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Both test email and password must be non-empty.");
        }

        return new TestCredentials(email, password);
    }
}
