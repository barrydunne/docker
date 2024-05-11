namespace Imaging.Application.Tests.Mocks;

internal class MockHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => MockSend(request, cancellationToken);

    // To be mocked
    public virtual Task<HttpResponseMessage> MockSend(HttpRequestMessage request, CancellationToken cancellationToken) => throw new NotImplementedException();
}
