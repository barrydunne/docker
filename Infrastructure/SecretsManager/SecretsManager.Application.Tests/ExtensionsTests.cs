namespace SecretsManager.Application.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Extensions")]
internal class ExtensionsTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void ToSecretVaultName_adds_prefix_and_suffix()
    {
        var name = _fixture.Create<string>();
        var expected = $"{Consts.SecretVaultPrefix}{name}{Consts.SecretVaultSuffix}";
        var actual = name.ToSecretVaultName();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void FromSecretVaultName_removes_prefix_and_suffix()
    {
        var name = _fixture.Create<string>();
        var full = $"{Consts.SecretVaultPrefix}{name}{Consts.SecretVaultSuffix}";
        var actual = full.FromSecretVaultName();
        Assert.That(actual, Is.EqualTo(name));
    }
}
