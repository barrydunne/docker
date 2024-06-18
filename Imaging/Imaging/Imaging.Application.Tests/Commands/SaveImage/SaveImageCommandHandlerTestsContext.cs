using AspNet.KickStarter.FunctionalResult;
using Imaging.Application.Commands.SaveImage;
using Imaging.Application.Queries.GetImageUrl;
using Imaging.Application.Tests.Mocks;
using MediatR;
using Microservices.Shared.CloudFiles;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using NSubstitute;
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
    private readonly ISender _mockMediator;
    private readonly ICloudFiles _mockCloudFiles;
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;
    private readonly IHttpClientFactory _mockHttpClientFactory;
    private readonly ISaveImageCommandHandlerMetrics _mockMetrics;
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

        _mockCloudFiles = Substitute.For<ICloudFiles>();
        _mockCloudFiles
            .UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => !_withUploadFailure)
            .AndDoes(callInfo =>
            {
                var container = callInfo.ArgAt<string>(0);
                var name = callInfo.ArgAt<string>(1);
                var content = callInfo.ArgAt<Stream>(2);
                Upload(container, name, content);
            });

        _mockHttpMessageHandler = Substitute.ForPartsOf<MockHttpMessageHandler>();
        _mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => GetImageResponse(callInfo.ArgAt<HttpRequestMessage>(0)));

        _mockHttpClientFactory = Substitute.For<IHttpClientFactory>();
        _mockHttpClientFactory
            .CreateClient(Arg.Any<string>())
            .Returns(callInfo => new HttpClient(_mockHttpMessageHandler));

        _mockMetrics = Substitute.For<ISaveImageCommandHandlerMetrics>();
        _mockLogger = new();
        _imageUrls = new();
        _invalidCoordinates = new();
        _exceptionCoordinates = new();

        _withExceptionMessage = null;
        _withDownloadFailure = false;
        _withUploadFailure = false;
        _validCoordinates = true;

        _mockMediator = Substitute.For<ISender>();
        _mockMediator
            .Send(Arg.Any<GetImageUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => _withExceptionMessage is not null ? throw new InvalidOperationException(_withExceptionMessage) : GetImageUrl((GetImageUrlQuery)callInfo.ArgAt<IRequest<Result<string?>>>(0)))
            .AndDoes(callInfo => _imageUrls[((GetImageUrlQuery)callInfo.ArgAt<IRequest<Result<string?>>>(0)).Coordinates] = $"http://{_fixture.Create<string>()}");

        _uploaded = new();
        _imageBytes = _fixture.Create<byte[]>();

        Sut = new(_mockQueue, _mockMediator, _mockCloudFiles, _mockHttpClientFactory, _mockMetrics, _mockLogger);
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
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertMetricsImagingTimeRecorded()
    {
        _mockMetrics.Received(1).RecordImagingTime(Arg.Any<double>());
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertMetricsDownloadTimeRecorded()
    {
        _mockMetrics.Received(1).RecordDownloadTime(Arg.Any<double>());
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertMetricsUploadTimeRecorded()
    {
        _mockMetrics.Received(1).RecordUploadTime(Arg.Any<double>());
        return this;
    }

    internal SaveImageCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
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
