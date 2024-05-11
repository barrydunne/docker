using Microservices.Shared.RestSharpFactory;
using RestSharp;

namespace Microservices.Shared.Mocks;

public class MockRestSharpFactory : IRestSharpClientFactory
{
    public MockRestClient MockRestClient { get; }

    public MockRestSharpFactory() => MockRestClient = new();

    public IRestClient CreateRestClient(RestClientOptions options, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false) => MockRestClient;
    public IRestClient CreateRestClient(ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false) => MockRestClient;
    public IRestClient CreateRestClient(Uri baseUrl, ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false) => MockRestClient;
    public IRestClient CreateRestClient(string baseUrl, ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null) => MockRestClient;
    public IRestClient CreateRestClient(HttpClient httpClient, RestClientOptions? options, bool disposeHttpClient = false, ConfigureSerialization? configureSerialization = null) => MockRestClient;
    public IRestClient CreateRestClient(HttpClient httpClient, bool disposeHttpClient = false, ConfigureRestClient? configureRestClient = null, ConfigureSerialization? configureSerialization = null) => MockRestClient;
    public IRestClient CreateRestClient(HttpMessageHandler handler, bool disposeHandler = true, ConfigureRestClient? configureRestClient = null, ConfigureSerialization? configureSerialization = null) => MockRestClient;
}
