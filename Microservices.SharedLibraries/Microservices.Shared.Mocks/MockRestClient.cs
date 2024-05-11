using RestSharp;
using RestSharp.Serializers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Mime;

namespace Microservices.Shared.Mocks;

public class MockRestClient : IRestClient
{

    private readonly ReadOnlyRestClientOptions _options;
    private readonly DefaultParameters _defaultParameters;
    private readonly RestSerializers _serializers;
    private readonly ConcurrentBag<RestRequest> _requests;

    private HttpStatusCode? _nextStatus;
    private bool _unknownHost;

    public ReadOnlyRestClientOptions Options => _options;

    public RestSerializers Serializers => _serializers;

    public DefaultParameters DefaultParameters => _defaultParameters;

    public IReadOnlyCollection<RestRequest> Requests => _requests;

    public static readonly (HttpStatusCode StatusCode, string? Content, string? ContentType) NotFoundResponse = new(HttpStatusCode.NotFound, null, MediaTypeNames.Application.Json);
    public Func<RestRequest, (HttpStatusCode StatusCode, string? Content, string? ContentType)> ExecuteRequest { get; set; } = (_) => NotFoundResponse;

    public MockRestClient()
    {
        _options = new ReadOnlyRestClientOptions(new RestClientOptions());
        _defaultParameters = new(_options);

        var serializerConfig = new SerializerConfig();
        serializerConfig.UseDefaultSerializers();
        _serializers = new RestSerializers(serializerConfig);
        _requests = new();

        _nextStatus = null;
        _unknownHost = false;

    }

    public Task<Stream?> DownloadStreamAsync(RestRequest request, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default)
         => Task.FromResult(GetResponse(request));

    public void Dispose() { }

    public void WithNextResponse(HttpStatusCode status) => _nextStatus = status;

    public void WithUnknownHost() => _unknownHost = true;

    private RestResponse GetResponse(RestRequest request)
    {
        _requests.Add(request);

        if (_unknownHost)
            return new RestResponse(request) { ResponseStatus = ResponseStatus.Error, ErrorMessage = "No such host is known", ErrorException = new HttpRequestException("No such host is known") };

        var (statusCode, content, contentType) = (_nextStatus is null) ? ExecuteRequest(request) : new(_nextStatus.Value, null, MediaTypeNames.Application.Json);
        return new RestResponse(request)
        {
            Content = content,
            ContentType = contentType,
            StatusCode = statusCode,
            IsSuccessStatusCode = (int)statusCode is >= 200 and <= 299,
            ResponseStatus = ResponseStatus.Completed
        };
    }
}
