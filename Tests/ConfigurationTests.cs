using NUnit.Framework;
using TestProject.Config;

namespace TestProject.Tests;

[TestFixture]
[Category("Unit")]
public class ConfigurationTests
{
    private static EnvironmentPolicy Policy => new()
    {
        AllowedHosts = new()
        {
            ["local"] = ["localhost", "127.0.0.1", "::1"],
            ["stage"] = ["stage.example.test"],
            ["prod"] = ["www.premieronline.com", "premieronline.com"]
        },
        ProductionHosts = ["www.premieronline.com", "premieronline.com"]
    };

    [TestCase(null)]
    [TestCase("")]
    [TestCase("unknown")]
    public void EnvironmentMustBeExplicitAndKnown(string? name) =>
        Assert.Throws<InvalidOperationException>(() => TestEnvironment.GetEnvironmentName(name, Policy));

    [Test]
    public void EnvironmentNameIsNormalized() =>
        Assert.That(TestEnvironment.GetEnvironmentName(" PROD ", Policy), Is.EqualTo("prod"));

    [TestCase("local", "http://localhost:8080")]
    [TestCase("local", "http://127.0.0.1:8080")]
    [TestCase("local", "http://[::1]:8080")]
    [TestCase("stage", "https://stage.example.test")]
    [TestCase("prod", "https://www.premieronline.com")]
    public void ConfiguredEnvironmentCanBeUsed(string name, string url)
    {
        var environment = TestEnvironment.Create(name, url, true, Policy);
        Assert.That(environment.Name, Is.EqualTo(name));
        Assert.That(environment.BaseUri, Is.EqualTo(new Uri(url)));
    }

    [Test]
    public void ProductionRequiresProcessOptIn() =>
        Assert.Throws<InvalidOperationException>(() =>
            TestEnvironment.Create("prod", "https://www.premieronline.com", false, Policy));

    [TestCase("stage", "https://www.premieronline.com")]
    [TestCase("local", "https://premieronline.com")]
    [TestCase("stage", "https://www.premieronline.com.")]
    [TestCase("stage", "http://stage.example.test")]
    [TestCase("stage", "https://other.example.test")]
    [TestCase("local", "http://other.example.test")]
    [TestCase("prod", "https://stage.example.test")]
    public void EnvironmentCannotUseAnUnapprovedHost(string name, string url) =>
        Assert.Throws<InvalidOperationException>(() => TestEnvironment.Create(name, url, true, Policy));

    [TestCase(null)]
    [TestCase("/relative")]
    [TestCase("file:///tmp/test")]
    [TestCase("https://user:password@stage.example.test")]
    [TestCase("https://stage.example.test?test=true")]
    [TestCase("https://stage.example.test#help")]
    public void InvalidBaseUrlIsRejected(string? url) =>
        Assert.Throws<InvalidOperationException>(() => TestEnvironment.Create("stage", url, true, Policy));
}
