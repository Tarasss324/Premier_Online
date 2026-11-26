using Bogus;

namespace TestProject.TestData;

internal sealed record RegistrationData(string FirstName, string LastName, string Email, string Password)
{
    private const string EmailPrefix = "qa";
    private const string EmailDomain = "mailinator.com";

    public static RegistrationData Create()
    {
        var faker = new Faker("en");
 
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
 
        var email = faker.Internet.Email(
            firstName,
            lastName,
            EmailDomain,
            uniqueSuffix: Guid.NewGuid().ToString("N")[..8]);
 
        return new RegistrationData(
            firstName,
            lastName,
            $"{EmailPrefix}.{email}".ToLowerInvariant(),
            GeneratePassword(faker));
    }

    private static string GeneratePassword(Faker faker)
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
 
        var chars = new List<char>
        {
            faker.Random.String2(1, upper)[0],
            faker.Random.String2(1, lower)[0],
            faker.Random.String2(1, digits)[0]
        };
        chars.AddRange(faker.Random.String2(5, upper + lower + digits));
 
        return new string(faker.Random.Shuffle(chars).ToArray());
    }
}

