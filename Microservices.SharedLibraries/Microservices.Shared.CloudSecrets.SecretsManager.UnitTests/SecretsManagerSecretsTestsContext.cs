using Microservices.Shared.Mocks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using RestSharp;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Microservices.Shared.CloudSecrets.SecretsManager.UnitTests;

internal class SecretsManagerSecretsTestsContext
{
    private readonly IOptions<SecretsManagerOptions> _mockOptions;
    private readonly MockRestSharpFactory _mockRestSharpFactory;
    private readonly MockLogger<SecretsManagerSecrets> _mockLogger;
    private readonly Dictionary<string, Dictionary<string, string>> _vaults;

    private string? _withExceptionMessage;

    internal SecretsManagerSecrets Sut => new(_mockOptions, _mockRestSharpFactory, _mockLogger);

    internal SecretsManagerSecretsTestsContext()
    {
        _mockOptions = Substitute.For<IOptions<SecretsManagerOptions>>();
        _mockOptions
            .Value
            .Returns(new SecretsManagerOptions { BaseUrl = "http://localhost" });
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
        Assert.That(_mockRestSharpFactory.MockRestClient.Requests, Has.Exactly(1).Items);
        return this;
    }

    internal SecretsManagerSecretsTestsContext AssertWarningLogged(string message)
    {
        Assert.That(_mockLogger.Messages, Does.Contain($"[{LogLevel.Warning}] {message}"));
        return this;
    }
}
