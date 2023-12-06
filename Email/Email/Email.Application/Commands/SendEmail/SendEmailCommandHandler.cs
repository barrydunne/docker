using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Email.Application.Models;
using Email.Application.Repositories;
using Email.Application.Templates;
using Microservices.Shared.CloudEmail;
using Microservices.Shared.CloudFiles;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace Email.Application.Commands.SendEmail;

/// <summary>
/// The handler for the <see cref="SendEmailCommand"/> command.
/// </summary>
internal class SendEmailCommandHandler : ICommandHandler<SendEmailCommand>
{
    private readonly ICloudEmail _cloudEmail;
    private readonly ICloudFiles _cloudFiles;
    private readonly IEmailRepository _emailRepository;
    private readonly ITemplateEngine _templateEngine;
    private readonly ISendEmailCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendEmailCommandHandler"/> class.
    /// </summary>
    /// <param name="cloudEmail">Used to send emails.</param>
    /// <param name="cloudFiles">Used to store image files in cloud storage.</param>
    /// <param name="emailRepository">The repository for saving email details.</param>
    /// <param name="templateEngine">The engine used to generate the HTML email body.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public SendEmailCommandHandler(ICloudEmail cloudEmail, ICloudFiles cloudFiles, IEmailRepository emailRepository, ITemplateEngine templateEngine, ISendEmailCommandHandlerMetrics metrics, ILogger<SendEmailCommandHandler> logger)
    {
        _cloudEmail = cloudEmail;
        _cloudFiles = cloudFiles;
        _emailRepository = emailRepository;
        _templateEngine = templateEngine;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(SendEmailCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(SendEmailCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Get image if available
            var images = new List<(string Cid, Stream Stream, string ContentType)>();
            string? imageCid = null;
            using var imageStream = new MemoryStream();
            if (command.Imaging.IsSuccessful)
            {
                var exists = await _cloudFiles.FileExistsAsync("Imaging", command.Imaging.ImagePath!, cancellationToken);
                if (!exists)
                    _logger.LogWarning("Image file does not exist. [{CorrelationId}]", command.JobId);
                else
                {
                    imageCid = Guid.NewGuid().ToString();
                    await _cloudFiles.DownloadFileAsync("Imaging", command.Imaging.ImagePath!, imageStream, cancellationToken);
                    imageStream.Seek(0, SeekOrigin.Begin);
                    var imageContentType = await GetImageMimeTypeAsync(imageStream, cancellationToken);
                    images.Add((imageCid!, imageStream!, imageContentType!));
                }
            }
            _metrics.RecordImageTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            // Generate email from template
            var html = _templateEngine.GenerateHtml(command.StartingAddress, command.DestinationAddress, command.Directions, command.Weather, imageCid);
            _metrics.RecordGenerateTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            _logger.LogDebug("Generated email. {HTML}. [{CorrelationId}]", html, command.JobId);

            // Send email
            var result = await _cloudEmail.SendEmailAsync($"Processing complete. Job {command.JobId}", html, null, new[] { command.Email }, null, null, images.ToArray());
            _metrics.RecordEmailTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            // Keep record of sent email
            await _emailRepository.InsertAsync(new SentEmail { JobId = command.JobId, RecipientEmail = command.Email, SentTime = DateTimeOffset.UtcNow }, cancellationToken);

            return result ? Result.Success() : Result.FromError("Failed to send email");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email. [{CorrelationId}]", command.JobId);
            return ex;
        }
    }

    private async Task<string> GetImageMimeTypeAsync(MemoryStream imageStream, CancellationToken cancellationToken)
    {
        var format = await Image.DetectFormatAsync(imageStream, cancellationToken);
        imageStream.Seek(0, SeekOrigin.Begin);
        return format.DefaultMimeType;
    }
}
