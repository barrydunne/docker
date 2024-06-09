using Imaging.Infrastructure.ExternalApi.Bing;
using Imaging.Infrastructure.ExternalApi.Bing.BingModels;
using Microservices.Shared.Mocks;
using RestSharp;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Imaging.Infrastructure.Tests.Bing;

internal class BingTestsContext
{
    private readonly Fixture _fixture;
    private readonly string _apiKey;
    private readonly MockCloudSecrets _mockCloudSecrets;
    private readonly MockRestSharpFactory _mockRestSharpFactory;
    private readonly MockRestSharpResiliencePipeline _mockRestSharpResiliencePipeline;
    private readonly MockLogger<BingApi> _mockLogger;
    private readonly ConcurrentDictionary<string, string> _knownImageUrls;

    private bool _withNoResults;
    private bool _withNoThumbnail;
    private bool _withNoValue;
    private bool _withBadRequest;
    private bool _withNoData;
    private string? _withExceptionMessage;

    internal BingApi Sut { get; }

    public BingTestsContext()
    {
        _fixture = new();
        _apiKey = _fixture.Create<string>();

        _mockCloudSecrets = new();
        _mockCloudSecrets.WithSecretValue("api.keys", "imaging.bing", _apiKey);

        _mockRestSharpFactory = new();
        _mockRestSharpFactory.MockRestClient.ExecuteRequest = ExecuteRequest;

        _mockRestSharpResiliencePipeline = new();
        _mockLogger = new();
        _knownImageUrls = new();
        _withNoResults = false;
        _withNoThumbnail = false;
        _withBadRequest = false;
        _withNoData = false;
        _withExceptionMessage = null;

        Sut = new(_mockCloudSecrets, _mockRestSharpFactory, _mockRestSharpResiliencePipeline, _mockLogger);
    }

    private (HttpStatusCode StatusCode, string? Content, string? ContentType) ExecuteRequest(RestRequest request)
    {
        if (_withExceptionMessage is not null)
            throw new InvalidOperationException(_withExceptionMessage);
        if ((request.Method == Method.Get) && (request.Resource == "/v7.0/images/search"))
            return GetImageResponse(request);
        return MockRestClient.NotFoundResponse;
    }

    /// <summary>
    /// Replicate the types of responses returned by Bing in different conditions.
    /// </summary>
    /// <param name="request">The API request.</param>
    /// <returns>The status and content for the response.</returns>
    private (HttpStatusCode StatusCode, string? Content, string? ContentType) GetImageResponse(RestRequest request)
    {
        if (_mockRestSharpFactory.MockRestClient.DefaultParameters.Where(_ => _.Type == ParameterType.HttpHeader).Select(_ => $"{_.Name}={_.Value}")?.FirstOrDefault() != $"Ocp-Apim-Subscription-Key={_apiKey}")
            return (HttpStatusCode.Unauthorized, """{"error":{"code":"401","message":"Access denied due to invalid subscription key or wrong API endpoint. Make sure to provide a valid key for an active subscription and use a correct regional API endpoint for your resource."}}""", MediaTypeNames.Application.Json);

        if (_withBadRequest)
            return (HttpStatusCode.BadRequest, """{"_type": "ErrorResponse", "instrumentation": {"_type": "ResponseInstrumentation"}, "errors": [{"code": "InvalidRequest", "subCode": "ParameterMissing", "message": "Required parameter is missing.", "moreDetails": "Required parameter is missing.", "parameter": "q", "value": ""}]}""", MediaTypeNames.Application.Json);

        if (_withNoData)
            return (HttpStatusCode.BadRequest, string.Empty, MediaTypeNames.Application.Json);

        var search = request.Parameters.FirstOrDefault(_ => (_.Type == ParameterType.QueryString) && (_.Name == "q"))?.Value?.ToString() ?? string.Empty;
        if (!_knownImageUrls.TryGetValue(search, out var imageUrl))
            imageUrl = _fixture.Create<string>();

        var value = new List<ImageObject>();
        if (!_withNoResults)
            value.Add(new ImageObject { ThumbnailUrl = _withNoThumbnail ? null : imageUrl });
        var images = new ImagesResponse { Value = _withNoValue ? null : value };
        return (HttpStatusCode.OK, JsonSerializer.Serialize(images), MediaTypeNames.Application.Json);
    }

    internal BingTestsContext WithImageUrl(string address, string imageUrl)
    {
        _knownImageUrls[address] = imageUrl;
        return this;
    }

    internal BingTestsContext WithNoResult()
    {
        _withNoResults = true;
        return this;
    }

    internal BingTestsContext WithNoThumbnail()
    {
        _withNoThumbnail = true;
        return this;
    }

    internal BingTestsContext WithNoValue()
    {
        _withNoValue = true;
        return this;
    }

    internal BingTestsContext WithBadRequest()
    {
        _withBadRequest = true;
        return this;
    }

    internal BingTestsContext WithNoData()
    {
        _withNoData = true;
        return this;
    }

    internal BingTestsContext WithSecretApiKey(string apiKey)
    {
        _mockCloudSecrets.WithSecretValue("api.keys", "imaging.bing", apiKey);
        return this;
    }

    internal BingTestsContext WithoutSecretApiKey()
    {
        _mockCloudSecrets.WithoutSecretValue("api.keys", "imaging.bing");
        return this;
    }

    internal BingTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }
}
