using Microservices.Shared.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Microservices.Shared.CloudSecrets.SecretsManager.UnitTests
{
    internal class SecretExtensionsTestsContext
    {
        private readonly Fixture _fixture;
        private readonly List<ServiceDescriptor> _serviceDescriptors;
        private readonly Mock<IServiceCollection> _mockServices;
        private readonly MockCloudSecrets _mockCloudSecrets;
        private readonly Mock<IConfigurationSection> _mockSecretConfigurationSection;
        private readonly Mock<IConfigurationSection> _mockConfigurationSection;
        private readonly Mock<IConfigurationManager> _mockConfigurationManager;
        private readonly Mock<IHostApplicationBuilder> _mockHostApplicationBuilder;

        internal IConfigurationSection ConfigurationSection => _mockConfigurationSection.Object;
        internal IHostApplicationBuilder Builder => _mockHostApplicationBuilder.Object;

        internal string Vault { get; }
        internal string Secret { get; }
        internal string SecretValue { get; }
        internal string ConfigurationKey { get; }

        public SecretExtensionsTestsContext() 
        {
            _fixture = new();
            Vault = _fixture.Create<string>();
            Secret = _fixture.Create<string>();
            SecretValue = _fixture.Create<string>();
            ConfigurationKey = _fixture.Create<string>();

            _serviceDescriptors = new();

            _mockServices = new(MockBehavior.Loose);
            _mockServices.Setup(_ => _.Add(It.IsAny<ServiceDescriptor>())).Callback((ServiceDescriptor serviceDescriptor) => _serviceDescriptors.Add(serviceDescriptor));
            _mockServices.Setup(_ => _.Count).Returns(() => _serviceDescriptors.Count);
            _mockServices.Setup(_ => _.CopyTo(It.IsAny<ServiceDescriptor[]>(), It.IsAny<int>())).Callback((ServiceDescriptor[] array, int index) => CopyServiceDescriptors(array, index));
            _mockServices.Setup(_ => _[It.IsAny<int>()]).Returns((int index) => _serviceDescriptors[index]);

            _mockCloudSecrets = new();
            _mockCloudSecrets.WithSecretValue(Vault, Secret, SecretValue);
            _mockServices.Object.AddSingleton(typeof(ICloudSecrets), _mockCloudSecrets.Object);

            _mockSecretConfigurationSection = new(MockBehavior.Loose);
            _mockSecretConfigurationSection.Setup(_ => _.Value).Verifiable();

            _mockConfigurationSection = new(MockBehavior.Loose);
            _mockConfigurationSection.Setup(_ => _.GetSection(ConfigurationKey))
                .Returns(() => _mockSecretConfigurationSection.Object);

            _mockConfigurationManager = new(MockBehavior.Loose);
            _mockConfigurationManager.Setup(_ => _.GetSection("SecretsManagerOptions")).Returns(_mockConfigurationSection.Object);

            _mockHostApplicationBuilder = new();
            _mockHostApplicationBuilder.Setup(_ => _.Services).Returns(_mockServices.Object);
            _mockHostApplicationBuilder.Setup(_ => _.Configuration).Returns(_mockConfigurationManager.Object);
        }

        private void CopyServiceDescriptors(ServiceDescriptor[] array, int index)
        {
            foreach (var service in _serviceDescriptors)
                array[index++] = service;
        }

        internal SecretExtensionsTestsContext AssertServiceAdded<TService, TImplementation>()
        {
            Assert.That(_serviceDescriptors, Has.Exactly(1).Matches<ServiceDescriptor>(_ => (_.ServiceType == typeof(TService)) && (_.ImplementationType == typeof(TImplementation))));
            return this;
        }

        internal SecretExtensionsTestsContext AssertServiceAdded<TService>()
        {
            Assert.That(_serviceDescriptors, Has.Exactly(1).Matches<ServiceDescriptor>(_ => _.ServiceType == typeof(TService)));
            return this;
        }

        internal SecretExtensionsTestsContext AssertConfigurationSectionHasSecretValue()
        {
            _mockSecretConfigurationSection.VerifySet(_ => _.Value = SecretValue, Times.Once);
            return this;
        }
    }
}
