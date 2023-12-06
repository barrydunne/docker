using Microservices.Shared.Mocks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RestSharp;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Microservices.Shared.CloudSecrets.SecretsManager.UnitTests;

internal class SecretsManagerSecretsTestsContext
{
    private readonly Mock<IOptions<SecretsManagerOptions>> _mockOptions;
    private readonly MockRestSharpFactory _mockRestSharpFactory;
    private readonly MockLogger<SecretsManagerSecrets> _mockLogger;
    private readonly Dictionary<string, Dictionary<string, string>> _vaults;

    private string? _withExceptionMessage;

    internal SecretsManagerSecrets Sut => new(_mockOptions.Object, _mockRestSharpFactory.Object, _mockLogger.Object);

    internal SecretsManagerSecretsTestsContext()
    {
        _mockOptions = new(MockBehavior.Strict);
        _mockOptions.Setup(_ => _.Value).Returns(new SecretsManagerOptions { BaseUrl = "http://localhost" });
        _mockRestSharpFactory = new();
        _mockRestSharpFactory.MockRestClient.ExecuteRequest = ExecuteRequest;
        _mockLogger = new();
        _vaults = new();
        _withExceptionMessage = null;
    }

    private (HttpStatusCode StatusCode, string? Content, string? ContentType) ExecuteRequest(RestRequest request)
    {
        if (_withExceptionMessage is not null)
            throw new InvalidOperationException(_withExceptionMessage);
        if ((request.Method == Method.Get) && Regex.IsMatch(request.Resource, "/secrets/vaults/[^/]+$"))
            return (HttpStatusCode.OK, GetVaultJson(request.Resource.Substring(16)), MediaTypeNames.Application.Json);
        return MockRestClient.NotFoundResponse;
    }

    private string? GetVaultJson(string vault)
    {
        if (_vaults.ContainsKey(vault))
            return JsonSerializer.Serialize(_vaults[vault]);
        return "{}";
    }

    internal SecretsManagerSecretsTestsContext WithVault(string vault, Dictionary<string, string> secrets)
    {
        _vaults[vault] = secrets;
        return this;
    }

    internal SecretsManagerSecretsTestsContext WithForbiddenResponse()
    {
        _mockRestSharpFactory.MockRestClient.WithNextResponse(HttpStatusCode.Forbidden);
        return this;
    }

    internal SecretsManagerSecretsTestsContext WithProblemResponse()
    {
        _mockRestSharpFactory.MockRestClient.WithNextResponse(HttpStatusCode.InternalServerError);
        return this;
    }

    internal SecretsManagerSecretsTestsContext WithUnknownHost()
    {
        _mockRestSharpFactory.MockRestClient.WithUnknownHost();
        return this;
    }

    internal SecretsManagerSecretsTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }

    internal SecretsManagerSecretsTestsContext AssertThatSecretsApiCalledOnce()
    {
        _mockRestSharpFactory.MockRestClient.Verify(_ => _.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()), Times.Once());
        return this;
    }

    internal SecretsManagerSecretsTestsContext AssertWarningLogged(string message)
    {
        Assert.That(_mockLogger.Log, Does.Contain($"[{LogLevel.Warning}] {message}"));
        return this;
    }
}
