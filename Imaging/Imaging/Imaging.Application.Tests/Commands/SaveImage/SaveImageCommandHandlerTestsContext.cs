using AspNet.KickStarter.CQRS;
using Imaging.Application.Commands.SaveImage;
using Imaging.Application.Queries.GetImageUrl;
using MediatR;
using Microservices.Shared.CloudFiles;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using Moq.Protected;
using NUnit.Framework.Constraints;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace Imaging.Application.Tests.Commands.SaveImage;

internal class SaveImageCommandHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly MockQueue<ImagingCompleteEvent> _mockQueue;
    private readonly Mock<ISender> _mockMediator;
    private readonly Mock<ICloudFiles> _mockCloudFiles;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<ISaveImageCommandHandlerMetrics> _mockMetrics;
    private readonly MockLogger<SaveImageCommandHandler> _mockLogger;
    private readonly ConcurrentBag<Coordinates> _invalidCoordinates;
    private readonly ConcurrentBag<Coordinates> _exceptionCoordinates;
    private readonly ConcurrentDictionary<Coordinates, string> _imageUrls;
    private readonly ConcurrentBag<(string Container, string Name, byte[] Bytes)> _uploaded;
    private readonly byte[] _imageBytes;

    private string? _withExceptionMessage;
    private bool _withDownloadFailure;
    private bool _withUploadFailure;
    private bool _validCoordinates;

    internal SaveImageCommandHandler Sut { get; }

    public SaveImageCommandHandlerTestsContext()
    {
        _fixture = new();
        _mockQueue = new();

        _mockCloudFiles = new(MockBehavior.Strict);
        _mockCloudFiles.Setup(_ => _.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback((string container, string name, Stream content, CancellationToken _) => Upload(container, name, content))
            .ReturnsAsync(() => !_withUploadFailure);

        _mockHttpMessageHandler = new(MockBehavior.Loose);
        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) => GetImageResponse(request));

        _mockHttpClientFactory = new(MockBehavior.Strict);
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient(_mockHttpMessageHandler.Object));

        _mockMetrics = new();
        _mockLogger = new();
        _imageUrls = new();
        _invalidCoordinates = new();
        _exceptionCoordinates = new();

        _withExceptionMessage = null;
        _withDownloadFailure = false;
        _withUploadFailure = false;
        _validCoordinates = true;

        _mockMediator = new();
        _mockMediator.Setup(_ => _.Send(It.IsAny<GetImageUrlQuery>(), It.IsAny<CancellationToken>()))
            .Callback((IRequest<Result<string?>> query, CancellationToken _) => _imageUrls[((GetImageUrlQuery)query).Coordinates] = $"http://{_fixture.Create<string>()}")
            .Returns((IRequest<Result<string?>> query, CancellationToken _) => _withExceptionMessage is not null ? throw new InvalidOperationException(_withExceptionMessage) : GetImageUrl((GetImageUrlQuery)query));

        _uploaded = new();
        _imageBytes = _fixture.Create<byte[]>();

        Sut = new(_mockQueue.Object, _mockMediator.Object, _mockCloudFiles.Object, _mockHttpClientFactory.Object, _mockMetrics.Object, _mockLogger.Object);
    }

    private void Upload(string container, string name, Stream content)
    {
        using var mem = new MemoryStream();
        content.CopyTo(mem);
        _uploaded.Add(new(container, name, mem.ToArray()));
    }

    private HttpResponseMessage GetImageResponse(HttpRequestMessage _)
    {
        if (_withDownloadFailure)
            return new HttpResponseMessage(HttpStatusCode.BadRequest);

        var content = new ByteArrayContent(_imageBytes);
        content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Image.Jpeg);
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
    }

    private Task<Result<string?>> GetImageUrl(GetImageUrlQuery query) => Task.Run(() =>
    {
        var address = query.Address;
        var coordinates = query.Coordinates;
        return _exceptionCoordinates.Contains(coordinates)
            ? throw new InvalidDataException(GetError(query))
            : _invalidCoordinates.Contains(coordinates)
                ? Result<string?>.FromError(GetError(query))
                : Result<string?>.Success(_imageUrls[coordinates]);
    });

    private string GetError(GetImageUrlQuery query) => $"Invalid: {query.Coordinates}";

    private string GetError(SaveImageCommand command) => $"Invalid: {command.Coordinates}";

    internal SaveImageCommandHandlerTestsContext WithInvalidCoordinates(SaveImageCommand command)
    {
        _validCoordinates = false;
        _invalidCoordinates.Add(command.Coordinates);
        return this;
    }

    internal SaveImageCommandHandlerTestsContext WithCoordinatesException(SaveImageCommand command)
    {
        _validCoordinates = false;
        _exceptionCoordinates.Add(command.Coordinates);
        return this;
    }

    internal SaveImageCommandHandlerTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }

    internal SaveImageCommandHandlerTestsContext WithDownloadFailure()
    {
        _withDownloadFailure = true;
        return this;
    }

    internal SaveImageCommandHandlerTestsContext WithUploadFailure()
    {
        _withUploadFailure = true;
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertMetricsImagingTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordImagingTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertMetricsDownloadTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordDownloadTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertMetricsUploadTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordUploadTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertImageUrlObtained(SaveImageCommand command)
    {
        Assert.That(_imageUrls.Keys, Does.Contain(command.Coordinates));
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertImageSavedToCloud()
    {
        Assert.That(_imageBytes.SequenceEqual(_uploaded.FirstOrDefault().Bytes));
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertImagingCompleteEventPublished(SaveImageCommand command)
        => AssertImagingCompleteEventPublished(command, Is.Not.Null);
    internal SaveImageCommandHandlerTestsContext AssertImagingCompleteEventNotPublished(SaveImageCommand command)
        => AssertImagingCompleteEventPublished(command, Is.Null);
    private SaveImageCommandHandlerTestsContext AssertImagingCompleteEventPublished(SaveImageCommand command, IResolveConstraint expression)
    {
        var published = _mockQueue.Messages.FirstOrDefault(_
            => _.JobId == command.JobId
            && _.Imaging.IsSuccessful == _validCoordinates
            && _.Imaging.Error == (_validCoordinates ? null : GetError(command))
            && _.Imaging.ImageUrl == (_validCoordinates ? _imageUrls[command.Coordinates] : null));
        Assert.That(published, expression);
        return this;
    }
}
