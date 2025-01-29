using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Microservices.Shared.CloudSecrets.Aws.UnitTests;

internal class AwsSecretsTestsContext
{
    private readonly Dictionary<string, string> _vaults;
    private readonly HashSet<string> _binaryVaults;
    private readonly IAmazonSecretsManager _mockAmazonSecretsManager;
    private readonly MockLogger<AwsSecrets> _mockLogger;

    private string? _withExceptionMessage;

    internal AwsSecrets Sut => new(_mockAmazonSecretsManager, _mockLogger);

    internal AwsSecretsTestsContext()
    {
        _vaults = new();
        _binaryVaults = new();

        _mockAmazonSecretsManager = Substitute.For<IAmazonSecretsManager>();
        _mockAmazonSecretsManager
            .GetSecretValueAsync(Arg.Any<GetSecretValueRequest>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var request = callInfo.Arg<GetSecretValueRequest>();
                var vault = request.SecretId;
                if (_vaults.ContainsKey(vault))
                {
                    return Task.FromResult(new GetSecretValueResponse
                    {
                        HttpStatusCode = HttpStatusCode.OK,
                        SecretString = _binaryVaults.Contains(vault) ? null : _vaults[vault],
                        SecretBinary = _binaryVaults.Contains(vault) ? new MemoryStream(Encoding.UTF8.GetBytes(_vaults[vault])) : null
                    });
                }
                throw new ResourceNotFoundException("Secrets Manager can't find the specified secret.");
            });

        _mockLogger = new();
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
            return _vaults[vault];
        return "{}";
    }

    internal AwsSecretsTestsContext WithVault(string vault, Dictionary<string, string> secrets)
    {
        _vaults[vault] = JsonSerializer.Serialize(secrets);
        return this;
    }

    internal AwsSecretsTestsContext WithBinaryVault(string vault, Dictionary<string, string> secrets)
    {
        _vaults[vault] = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(secrets)));
        _binaryVaults.Add(vault);
        return this;
    }

    internal AwsSecretsTestsContext WithBlankVault(string vault)
    {
        _vaults[vault] = "";
        return this;
    }

    internal AwsSecretsTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }

    internal AwsSecretsTestsContext WithUnsuccessfulResponse()
    {
        _mockAmazonSecretsManager
            .GetSecretValueAsync(Arg.Any<GetSecretValueRequest>(), Arg.Any<CancellationToken>())
            .Returns(new GetSecretValueResponse { HttpStatusCode = HttpStatusCode.BadRequest });
        return this;
    }

    internal AwsSecretsTestsContext AssertThatSecretsApiCalledOnce()
    {
        _mockAmazonSecretsManager.Received(1).GetSecretValueAsync(Arg.Any<GetSecretValueRequest>(), Arg.Any<CancellationToken>());
        return this;
    }

    internal AwsSecretsTestsContext AssertErrorLogged(string message)
    {
        _mockLogger.Messages.ShouldContain($"[{LogLevel.Error}] {message}");
        return this;
    }
}
