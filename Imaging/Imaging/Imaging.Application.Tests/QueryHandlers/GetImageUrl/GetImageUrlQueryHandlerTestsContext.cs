using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using Imaging.Application.ExternalApi;
using Imaging.Application.Queries.GetImageUrl;

namespace Imaging.Application.Tests.QueryHandlers.GetImageUrl;

internal class GetImageUrlQueryHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly Mock<IExternalApi> _mockExternalService;
    private readonly Mock<IGetImageUrlQueryHandlerMetrics> _mockMetrics;
    private readonly MockLogger<GetImageUrlQueryHandler> _mockLogger;

    private string? _imageUrl;
    private string? _withExceptionMessage;

    internal GetImageUrlQueryHandler Sut { get; }

    public GetImageUrlQueryHandlerTestsContext()
    {
        _fixture = new();
        _mockMetrics = new();
        _mockLogger = new();

        _imageUrl = null;
        _withExceptionMessage = null;

        _mockExternalService = new();
        _mockExternalService.Setup(_ => _.GetImageUrlAsync(It.IsAny<string>(), It.IsAny<Coordinates>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => GetImageUrl());

        Sut = new(_mockExternalService.Object, _mockMetrics.Object, _mockLogger.Object);
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
        _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
        return this;
    }

    internal GetImageUrlQueryHandlerTestsContext AssertMetricsExternalTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordExternalTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
