using Microservices.Shared.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace Microservices.Shared.CloudSecrets.UnitTests;

internal class ServiceExtensionsTestsContext
{
    private readonly Fixture _fixture;
    private readonly List<ServiceDescriptor> _serviceDescriptors;
    private readonly IServiceCollection _mockServices;
    private readonly MockCloudSecrets _mockCloudSecrets;
    private readonly IConfigurationSection _mockSecretConfigurationSection;
    private readonly IConfigurationSection _mockConfigurationSection;
    private readonly IHostApplicationBuilder _mockHostApplicationBuilder;

    internal IConfigurationSection ConfigurationSection => _mockConfigurationSection;
    internal IHostApplicationBuilder Builder => _mockHostApplicationBuilder;

    internal string Vault { get; }
    internal string Secret { get; }
    internal string SecretValue { get; }
    internal string ConfigurationKey { get; }

    public ServiceExtensionsTestsContext() 
    {
        _fixture = new();
        Vault = _fixture.Create<string>();
        Secret = _fixture.Create<string>();
        SecretValue = _fixture.Create<string>();
        ConfigurationKey = _fixture.Create<string>();

        _serviceDescriptors = new();

        _mockServices = Substitute.For<IServiceCollection>();
        _mockServices
            .When(_ => _.Add(Arg.Any<ServiceDescriptor>()))
            .Do(callInfo => _serviceDescriptors.Add(callInfo.ArgAt<ServiceDescriptor>(0)));
        _mockServices
            .Count
            .Returns(callInfo => _serviceDescriptors.Count);
        _mockServices
            .When(_ => _.CopyTo(Arg.Any<ServiceDescriptor[]>(), Arg.Any<int>()))
            .Do(callInfo => CopyServiceDescriptors(callInfo.ArgAt<ServiceDescriptor[]>(0), callInfo.ArgAt<int>(1)));

        _mockCloudSecrets = new();
        _mockCloudSecrets.WithSecretValue(Vault, Secret, SecretValue);
        _mockServices.AddSingleton(typeof(ICloudSecrets), _mockCloudSecrets);

        _mockSecretConfigurationSection = Substitute.For<IConfigurationSection>();

        _mockConfigurationSection = Substitute.For<IConfigurationSection>();
        _mockConfigurationSection
            .GetSection(ConfigurationKey)
            .Returns(callInfo => _mockSecretConfigurationSection);

        _mockHostApplicationBuilder = Substitute.For<IHostApplicationBuilder>();
        _mockHostApplicationBuilder
            .Services
            .Returns(callInfo => _mockServices);
    }

    private void CopyServiceDescriptors(ServiceDescriptor[] array, int index)
    {
        foreach (var service in _serviceDescriptors)
            array[index++] = service;
    }

    internal ServiceExtensionsTestsContext AssertConfigurationSectionHasSecretValue()
    {
        _mockSecretConfigurationSection.Received(1).Value = SecretValue;
        return this;
    }
}
