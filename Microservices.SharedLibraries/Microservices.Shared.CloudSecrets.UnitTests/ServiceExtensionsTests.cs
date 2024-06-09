namespace Microservices.Shared.CloudSecrets.UnitTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Extensions")]
internal class ServiceExtensionsTests
{
    private readonly ServiceExtensionsTestsContext _context = new();

    [Test]
    public void SecretExtensions_ApplySecret_sets_config_value()
    {
        ServiceExtensions.ApplySecret(_context.ConfigurationSection, _context.Builder, _context.ConfigurationKey, _context.Vault, _context.Secret);
        _context.AssertConfigurationSectionHasSecretValue();
    }
}
