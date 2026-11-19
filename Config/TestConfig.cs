using System.Collections;
using System.Text.Json;
using DotNetEnv;

namespace TestProject.Config;

/// <summary>
/// Loads one immutable configuration snapshot. Process variables override the optional
/// project-local .env file; the effective URL is validated before UI setup.
/// </summary>
public static class TestConfig
{
    private static readonly Lazy<Configuration> Current = new(Load);

    public static string EnvName => Current.Value.Environment.Name;
    public static string BaseUrl => Current.Value.Environment.BaseUri.AbsoluteUri.TrimEnd('/');
    public static string UserEmail => GetRequired("TEST_USER_EMAIL");
    public static string UserPassword => GetRequired("TEST_USER_PASSWORD");

    internal static string? GetOptional(string key) => Current.Value.Values.GetValueOrDefault(key);

    private static Configuration Load()
    {
        var policyFile = Path.Combine(AppContext.BaseDirectory, "Config", "environments.json");
        var policy = JsonSerializer.Deserialize<EnvironmentPolicy>(File.ReadAllText(policyFile))
            ?? throw new InvalidOperationException("Environment policy is missing.");
        var environmentName = TestEnvironment.GetEnvironmentName(
            Environment.GetEnvironmentVariable("TEST_ENV"), policy);

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var projectDirectory = FindProjectDirectory();
        var environmentFile = projectDirectory is null
            ? null
            : Path.Combine(projectDirectory, $".env.{environmentName}");

        if (environmentFile is not null && File.Exists(environmentFile))
        {
            foreach (var pair in Env.NoEnvVars().Load(environmentFile))
            {
                values[pair.Key] = pair.Value;
            }
        }

        foreach (DictionaryEntry pair in Environment.GetEnvironmentVariables())
        {
            if (pair.Key is string key && pair.Value is string value)
            {
                values[key] = value;
            }
        }

        var environment = TestEnvironment.Create(
            environmentName,
            values.GetValueOrDefault("BASE_URL"),
            string.Equals(Environment.GetEnvironmentVariable("ALLOW_PRODUCTION_TESTS"),
                "true", StringComparison.OrdinalIgnoreCase),
            policy);

        return new Configuration(environment, values);
    }

    private static string GetRequired(string key)
    {
        if (!Current.Value.Values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Variable '{key}' is required for environment '{EnvName}'. " +
                $"Set it in the process environment or the project-local .env.{EnvName} file.");
        }

        return value;
    }

    private static string? FindProjectDirectory()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "TestProject.csproj")))
            {
                return directory.FullName;
            }
        }

        return null;
    }

    private sealed record Configuration(
        TestEnvironment Environment,
        IReadOnlyDictionary<string, string> Values);
}
