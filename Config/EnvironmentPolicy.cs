namespace TestProject.Config;

internal sealed class EnvironmentPolicy
{
    public Dictionary<string, string[]> AllowedHosts { get; init; } = new(StringComparer.Ordinal);
    public string[] ProductionHosts { get; init; } = [];
}
