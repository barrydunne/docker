using Microsoft.Extensions.Options;

namespace Microservices.Shared.CloudSecrets.SecretsManager.UnitTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Extensions")]
internal class SecretExtensionsTests
{
    private readonly SecretExtensionsTestsContext _context = new();

    [Test]
    public void SecretExtensions_AddSecretsManagerSecrets_adds_ICloudSecrets()
    {
        SecretExtensions.AddSecretsManagerSecrets(_context.Builder);
        _context.AssertServiceAdded<ICloudSecrets, SecretsManagerSecrets>();
    }

    [Test]
    public void SecretExtensions_AddSecretsManagerSecrets_configures_SecretsManagerOptions()
    {
        SecretExtensions.AddSecretsManagerSecrets(_context.Builder);
        _context.AssertServiceAdded<IConfigureOptions<SecretsManagerOptions>>();
    }

    [Test]
    public void SecretExtensions_ApplySecret_sets_config_value()
    {
        SecretExtensions.ApplySecret(_context.ConfigurationSection, _context.Builder, _context.ConfigurationKey, _context.Vault, _context.Secret);
        _context.AssertConfigurationSectionHasSecretValue();
    }
}
