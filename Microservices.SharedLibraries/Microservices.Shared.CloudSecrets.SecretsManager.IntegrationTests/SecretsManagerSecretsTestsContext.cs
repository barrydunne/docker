using Microservices.Shared.Mocks;
using Microservices.Shared.RestSharpFactory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests;

internal class SecretsManagerSecretsTestsContext
{
    private readonly Mock<IOptions<SecretsManagerOptions>> _mockOptions;
    private readonly MockLogger<SecretsManagerSecrets> _mockLogger;

    internal SecretsManagerSecrets Sut => new(_mockOptions.Object, new RestSharpClientFactory(), _mockLogger.Object);

    internal SecretsManagerSecretsTestsContext()
    {
        _mockOptions = new(MockBehavior.Strict);
        _mockOptions.Setup(_ => _.Value).Returns(new SecretsManagerOptions { BaseUrl = "http://localhost:10083" });

        _mockLogger = new();
    }

    internal SecretsManagerSecretsTestsContext WithForbiddenResponse()
    {
        // This uses a local running copy of https://httpstat.us to remove the dependency on external site availability
        // docker run -p 8888:80 -d --name http-status ghcr.io/aaronpowell/httpstatus:c82331cbde67f430da66a84a758d94ba5afd7620
        _mockOptions.Setup(_ => _.Value).Returns(new SecretsManagerOptions { BaseUrl = "http://localhost:8888/403" });
        return this;
    }

    internal SecretsManagerSecretsTestsContext WithProblemResponse()
    {
        // This uses a local running copy of https://httpstat.us to remove the dependency on external site availability
        // docker run -p 8888:80 -d --name http-status ghcr.io/aaronpowell/httpstatus:c82331cbde67f430da66a84a758d94ba5afd7620
        _mockOptions.Setup(_ => _.Value).Returns(new SecretsManagerOptions { BaseUrl = "http://localhost:8888/505" });
        return this;
    }

    internal SecretsManagerSecretsTestsContext WithUnknownHost()
    {
        _mockOptions.Setup(_ => _.Value).Returns(new SecretsManagerOptions { BaseUrl = "http://-0" });
        return this;
    }

    internal SecretsManagerSecretsTestsContext AssertWarningLogged(string message)
    {
        Assert.That(_mockLogger.Log, Does.Contain($"[{LogLevel.Warning}] {message}"));
        return this;
    }
}
