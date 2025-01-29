using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Imaging.Application.ExternalApi;
using Imaging.Application.Queries.GetImageUrl;

namespace Imaging.Application.Tests.QueryHandlers.GetImageUrl;

internal class GetImageUrlQueryHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly IExternalApi _mockExternalService;
    private readonly IGetImageUrlQueryHandlerMetrics _mockMetrics;
    private readonly MockLogger<GetImageUrlQueryHandler> _mockLogger;

    private string? _imageUrl;
    private string? _withExceptionMessage;

    internal GetImageUrlQueryHandler Sut { get; }

    public GetImageUrlQueryHandlerTestsContext()
    {
        _fixture = new();
        _mockMetrics = Substitute.For<IGetImageUrlQueryHandlerMetrics>();
        _mockLogger = new();

        _imageUrl = null;
        _withExceptionMessage = null;

        _mockExternalService = Substitute.For<IExternalApi>();
        _mockExternalService
            .GetImageUrlAsync(Arg.Any<string>(), Arg.Any<Coordinates>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => GetImageUrl());

        Sut = new(_mockExternalService, _mockMetrics, _mockLogger);
    }

    private string? GetImageUrl()
    {
        if (_withExceptionMessage is not null)
            throw new InvalidOperationException(_withExceptionMessage);
        return _imageUrl ?? _fixture.Create<string>();
    }

    internal GetImageUrlQueryHandlerTestsContext WithExternalResult(string imageUrl)
    {
        _imageUrl = imageUrl;
        return this;
    }

    internal GetImageUrlQueryHandlerTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }

    internal GetImageUrlQueryHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal GetImageUrlQueryHandlerTestsContext AssertMetricsExternalTimeRecorded()
    {
        _mockMetrics.Received(1).RecordExternalTime(Arg.Any<double>());
        return this;
    }
}
