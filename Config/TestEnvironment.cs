namespace TestProject.Config;

/// <summary>
/// Validates the effective environment and URL without network access or process mutation.
/// Production requires both TEST_ENV=prod and a per-process opt-in.
/// </summary>
internal sealed class TestEnvironment
{
    private TestEnvironment(string name, Uri baseUri)
    {
        Name = name;
        BaseUri = baseUri;
    }

    public string Name { get; }
    public Uri BaseUri { get; }

    internal static string GetEnvironmentName(string? value, EnvironmentPolicy policy)
    {
        var name = value?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(name) || !policy.AllowedHosts.ContainsKey(name))
        {
            throw new InvalidOperationException(
                "Set TEST_ENV explicitly to an environment listed in Config/environments.json.");
        }

        return name;
    }

    internal static TestEnvironment Create(
        string? environmentName,
        string? baseUrl,
        bool allowProductionTests,
        EnvironmentPolicy policy)
    {
        var name = GetEnvironmentName(environmentName, policy);
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)
            || !string.IsNullOrEmpty(uri.UserInfo)
            || !string.IsNullOrEmpty(uri.Query)
            || !string.IsNullOrEmpty(uri.Fragment))
        {
            throw new InvalidOperationException(
                "BASE_URL must be an absolute HTTP(S) URL without credentials, query or fragment.");
        }

        if (uri.Scheme != Uri.UriSchemeHttps && (name != "local" || !uri.IsLoopback))
        {
            throw new InvalidOperationException("Only the local loopback environment may use HTTP.");
        }

        var host = NormalizeHost(uri.IdnHost);
        var isProduction = policy.ProductionHosts.Any(value => NormalizeHost(value) == host);
        if (policy.ProductionHosts.Length == 0)
        {
            throw new InvalidOperationException("Production hosts must be defined in the environment policy.");
        }

        if ((name == "prod" || isProduction) &&
            (name != "prod" || !isProduction || !allowProductionTests))
        {
            throw new InvalidOperationException(
                "Production requires TEST_ENV=prod and ALLOW_PRODUCTION_TESTS=true in the process environment. " +
                "A production URL cannot be used as local or stage.");
        }

        if (!policy.AllowedHosts[name].Any(value => NormalizeHost(value) == host))
        {
            throw new InvalidOperationException(
                $"BASE_URL host is not allowed for '{name}'. Configure Config/environments.json.");
        }

        return new TestEnvironment(name, uri);
    }

    private static string NormalizeHost(string value) =>
        value.Trim().TrimEnd('.').Trim('[', ']').ToLowerInvariant();
}
