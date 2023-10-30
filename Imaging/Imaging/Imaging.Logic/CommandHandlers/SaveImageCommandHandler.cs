using Ardalis.GuardClauses;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using CSharpFunctionalExtensions;
using Imaging.Logic.Commands;
using Imaging.Logic.Metrics;
using Imaging.Logic.Queries;
using MediatR;
using Microservices.Shared.CloudFiles;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Imaging.Logic.CommandHandlers
{
    /// <summary>
    /// The handler for the <see cref="SaveImageCommand"/> command.
    /// </summary>
    internal class SaveImageCommandHandler : ICommandHandler<SaveImageCommand>
    {
        private readonly IQueue<ImagingCompleteEvent> _completeQueue;
        private readonly IMediator _mediator;
        private readonly ICloudFiles _cloudFiles;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISaveImageCommandHandlerMetrics _metrics;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveImageCommandHandler"/> class.
        /// </summary>
        /// <param name="completeQueue">The queue for publishing <see cref="ImagingCompleteEvent"/> events to.</param>
        /// <param name="mediator">The mediator to send commands and queries to.</param>
        /// <param name="cloudFiles">Used to store image files in cloud storage.</param>
        /// <param name="httpClientFactory">The factory to create HttpClients.</param>
        /// <param name="metrics">The metrics provider for this handler.</param>
        /// <param name="logger">The logger to write to.</param>
        public SaveImageCommandHandler(IQueue<ImagingCompleteEvent> completeQueue, IMediator mediator, ICloudFiles cloudFiles, IHttpClientFactory httpClientFactory, ISaveImageCommandHandlerMetrics metrics, ILogger<SaveImageCommandHandler> logger)
        {
            _completeQueue = completeQueue;
            _mediator = mediator;
            _cloudFiles = cloudFiles;
            _httpClientFactory = httpClientFactory;
            _metrics = metrics;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Result> Handle(SaveImageCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(SaveImageCommand), command.JobId);
            _metrics.IncrementCount();

            var stopwatch = Stopwatch.StartNew();

            var guardResult = PerformGuardChecks(command);
            _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            if (guardResult.IsFailure)
                return Result.Failure<Guid>(guardResult.Error);

            try
            {
                var imagingTask = _mediator.Send(new GetImageUrlQuery(command.JobId, command.Address, command.Coordinates), cancellationToken);
                try
                {
                    await imagingTask;
                }
                catch (Exception ex)
                {
                    // This should not happen due to exception handling in the query handler that will return a failed Result instead of throwing an exception. Added for safety.
                    _logger.LogError(ex, "Unexpected error waiting for image. [{CorrelationId}]", command.JobId);
                }
                var imagingResult = imagingTask.GetTaskResult();
                _metrics.RecordImagingTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

                var imagePath = string.Empty;
                var uploaded = false;
                if (imagingResult.IsSuccess)
                {
                    // Get file from internet
                    _logger.LogInformation("Downloading {ImageUrl}. [{CorrelationId}]", imagingResult.Value, command.JobId);
                    using var httpClient = _httpClientFactory.CreateClient();
                    using var response = await httpClient.GetAsync(imagingResult.Value);
                    response.EnsureSuccessStatusCode();
                    _metrics.RecordDownloadTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

                    // Stream to cloud storage
                    imagePath = command.JobId.ToString("N");
                    _logger.LogInformation("Streaming to cloud storage {Path}. [{CorrelationId}]", imagePath, command.JobId);
                    using var imageStream = await response.Content.ReadAsStreamAsync();
                    uploaded = await _cloudFiles.UploadFileAsync("Imaging", imagePath, imageStream, cancellationToken);
                    _metrics.RecordUploadTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
                    _logger.LogInformation("Upload to cloud storage {Path} result {Result}. [{CorrelationId}]", imagePath, uploaded ? "success" : "failure", command.JobId);
                }

                var completeEvent = CreateImagingCompleteEvent(command, imagingResult, uploaded, imagePath);
                await PublishEventAsync(command, completeEvent, cancellationToken);
                _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get image. [{CorrelationId}]", command.JobId);
                return Result.Failure(ex.Message);
            }
        }

        private Result PerformGuardChecks(SaveImageCommand command)
        {
            try
            {
                Guard.Against.NullOrEmpty(command.JobId, nameof(SaveImageCommand.JobId));
                Guard.Against.Null(command.Address, nameof(SaveImageCommand.Address));
                Guard.Against.Null(command.Coordinates, nameof(SaveImageCommand.Coordinates));
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to validate command properties. [{CorrelationId}]", command.JobId);
                return Result.Failure(ex.Message);
            }
        }

        private ImagingCompleteEvent CreateImagingCompleteEvent(SaveImageCommand command, Result<string?> result, bool uploaded, string imagePath)
        {
            var imagingResult = result.IsSuccess && uploaded
                ? new ImagingResult(true, result.Value, imagePath, null)
                : new ImagingResult(false, null, null, result.Error);
            return new(command.JobId, imagingResult);
        }

        private async Task PublishEventAsync(SaveImageCommand command, ImagingCompleteEvent completeEvent, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Publishing imaging complete event. {Event} [{CorrelationId}]", completeEvent, command.JobId);
            await _completeQueue.PublishAsync(completeEvent, cancellationToken);
            _logger.LogInformation("Published imaging complete event. [{CorrelationId}]", command.JobId);
        }
    }
}
