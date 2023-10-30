using Microservices.Shared.RestSharpFactory;
using Moq;
using RestSharp;

namespace Microservices.Shared.Mocks
{
    public class MockRestSharpFactory : Mock<IRestSharpClientFactory>
    {
        public MockRestClient MockRestClient { get; }

        public MockRestSharpFactory(MockBehavior behavior = MockBehavior.Strict) : base(behavior)
        {
            MockRestClient = new();

            Setup(_ => _.CreateRestClient(It.IsAny<RestClientOptions>(), It.IsAny<ConfigureHeaders?>(), It.IsAny<ConfigureSerialization?>(), It.IsAny<bool>()))
                .Returns(MockRestClient.Object);

            Setup(_ => _.CreateRestClient(It.IsAny<ConfigureRestClient?>(), It.IsAny<ConfigureHeaders?>(), It.IsAny<ConfigureSerialization?>(), It.IsAny<bool>()))
                .Returns(MockRestClient.Object);

            Setup(_ => _.CreateRestClient(It.IsAny<Uri>(), It.IsAny<ConfigureRestClient?>(), It.IsAny<ConfigureHeaders?>(), It.IsAny<ConfigureSerialization>(), It.IsAny<bool>()))
                .Returns(MockRestClient.Object);

            Setup(_ => _.CreateRestClient(It.IsAny<string>(), It.IsAny<ConfigureRestClient?>(), It.IsAny<ConfigureHeaders?>(), It.IsAny<ConfigureSerialization?>()))
                .Returns(MockRestClient.Object);

            Setup(_ => _.CreateRestClient(It.IsAny<HttpClient>(), It.IsAny<RestClientOptions?>(), It.IsAny<bool>(), It.IsAny<ConfigureSerialization?>()))
                .Returns(MockRestClient.Object);

            Setup(_ => _.CreateRestClient(It.IsAny<HttpClient>(), It.IsAny<bool>(), It.IsAny<ConfigureRestClient?>(), It.IsAny<ConfigureSerialization?>()))
                .Returns(MockRestClient.Object);

            Setup(_ => _.CreateRestClient(It.IsAny<HttpMessageHandler>(), It.IsAny<bool>(), It.IsAny<ConfigureRestClient?>(), It.IsAny<ConfigureSerialization?>()))
                .Returns(MockRestClient.Object);
        }
    }
}
