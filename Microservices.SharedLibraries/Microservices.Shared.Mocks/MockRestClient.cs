using Moq;
using RestSharp;
using RestSharp.Serializers;
using System.Net;
using System.Net.Mime;

namespace Microservices.Shared.Mocks
{
    public class MockRestClient : Mock<IRestClient>
    {
        private readonly ReadOnlyRestClientOptions _options;
        private readonly DefaultParameters _defaultParameters;
        private readonly RestSerializers _serializers;
        private HttpStatusCode? _nextStatus;
        private bool _unknownHost;

        public static readonly (HttpStatusCode StatusCode, string? Content, string? ContentType) NotFoundResponse = new (HttpStatusCode.NotFound, null, MediaTypeNames.Application.Json);
        public Func<RestRequest, (HttpStatusCode StatusCode, string? Content, string? ContentType)> ExecuteRequest { get; set; } = (_) => NotFoundResponse;

        public MockRestClient(MockBehavior behavior = MockBehavior.Strict) : base(behavior)
        {
            _options = new ReadOnlyRestClientOptions(new RestClientOptions());
            _defaultParameters = new(_options);

            var serializerConfig = new SerializerConfig();
            serializerConfig.UseDefaultSerializers();
            _serializers = new RestSerializers(serializerConfig);

            _nextStatus = null;
            _unknownHost = false;

            Setup(_ => _.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RestRequest request, CancellationToken _) => GetResponse(request));

            Setup(_ => _.DefaultParameters)
                .Returns(() => _defaultParameters);

            Setup(_ => _.Options)
                .Returns(() => _options);

            Setup(_ => _.Serializers)
                .Returns(() => _serializers);

            Setup(_ => _.Dispose()).Verifiable();
        }

        public DefaultParameters DefaultParameters => _defaultParameters;

        public void WithNextResponse(HttpStatusCode status) => _nextStatus = status;

        public void WithUnknownHost() => _unknownHost = true;

        private RestResponse GetResponse(RestRequest request)
        {
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
}
