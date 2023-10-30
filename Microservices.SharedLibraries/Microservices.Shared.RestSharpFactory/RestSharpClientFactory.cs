using RestSharp;

namespace Microservices.Shared.RestSharpFactory
{
    /// <inheritdoc/>
    public class RestSharpClientFactory : IRestSharpClientFactory
    {
        /// <inheritdoc/>
        public IRestClient CreateRestClient(RestClientOptions options, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false)
            => new RestClient(options, configureDefaultHeaders, configureSerialization, useClientFactory);

        /// <inheritdoc/>
        public IRestClient CreateRestClient(ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false)
            => new RestClient(configureRestClient, configureDefaultHeaders, configureSerialization, useClientFactory);

        /// <inheritdoc/>
        public IRestClient CreateRestClient(Uri baseUrl, ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false)
            => new RestClient(baseUrl, configureRestClient, configureDefaultHeaders, configureSerialization, useClientFactory);

        /// <inheritdoc/>
        public IRestClient CreateRestClient(string baseUrl, ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null)
            => new RestClient(baseUrl, configureRestClient, configureDefaultHeaders, configureSerialization);

        /// <inheritdoc/>
        public IRestClient CreateRestClient(HttpClient httpClient, RestClientOptions? options, bool disposeHttpClient = false, ConfigureSerialization? configureSerialization = null)
            => new RestClient(httpClient, options, disposeHttpClient, configureSerialization);

        /// <inheritdoc/>
        public IRestClient CreateRestClient(HttpClient httpClient, bool disposeHttpClient = false, ConfigureRestClient? configureRestClient = null, ConfigureSerialization? configureSerialization = null)
            => new RestClient(httpClient, disposeHttpClient, configureRestClient, configureSerialization);

        /// <inheritdoc/>
        public IRestClient CreateRestClient(HttpMessageHandler handler, bool disposeHandler = true, ConfigureRestClient? configureRestClient = null, ConfigureSerialization? configureSerialization = null)
            => new RestClient(handler, disposeHandler, configureRestClient, configureSerialization);
    }
}
