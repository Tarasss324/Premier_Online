namespace TestProject.TestData;

internal sealed record RegistrationData(string FirstName, string LastName, string Email, string Password)
{
    public static RegistrationData Create() =>
        new("Test", "User", $"test-{Guid.NewGuid():N}@example.com", "TestPass123");
}
