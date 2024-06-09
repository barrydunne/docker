using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Ardalis.GuardClauses;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Diagnostics;
using System.Net.Mail;
using System.Net.Mime;

namespace Microservices.Shared.CloudEmail.Aws;

/// <inheritdoc/>
public class AwsEmail : ICloudEmail
{
    private readonly AwsEmailOptions _options;
    private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;
    private readonly ILogger _logger;

    private static readonly ActivitySource _activitySource = new(CloudEmail.ActivitySourceName);

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmail"/> class.
    /// </summary>
    /// <param name="options">The connection configuration options.</param>
    /// <param name="amazonSimpleEmailService">The client to use to send emails.</param>
    /// <param name="logger">The logger to write to.</param>
    public AwsEmail(IOptions<AwsEmailOptions> options, IAmazonSimpleEmailService amazonSimpleEmailService, ILogger<AwsEmail> logger)
    {
        _options = options.Value;
        _amazonSimpleEmailService = amazonSimpleEmailService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<bool> SendEmailAsync(string subject, string htmlBody, params string[] to)
        => await SendEmailAsync(subject, htmlBody, null, to, null, null);

    /// <inheritdoc/>
    public async Task<bool> SendEmailAsync(string subject, string? htmlBody, string? plainBody, string[] to, string[]? cc, string[]? bcc, params (string Cid, Stream Stream, string ContentType)[] images)
    {
        using var activity = _activitySource.StartActivity("SES Send email", ActivityKind.Client);
        Guard.Against.Null(htmlBody ?? plainBody, nameof(htmlBody), $"Must provide either {nameof(htmlBody)} or {nameof(plainBody)}");
        Guard.Against.Empty(to, nameof(to));

        try
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_options.From),
                Subject = subject
            };
            foreach (var recipient in to)
                mail.To.Add(new MailAddress(recipient));
            if (cc is not null)
            {
                foreach (var recipient in cc)
                    mail.CC.Add(new MailAddress(recipient));
            }
            if (bcc is not null)
            {
                foreach (var recipient in bcc)
                    mail.Bcc.Add(new MailAddress(recipient));
            }
            if (!string.IsNullOrWhiteSpace(plainBody))
                mail.Body = plainBody;
            if (!string.IsNullOrWhiteSpace(htmlBody))
            {
                var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                if (images is not null)
                {
                    foreach (var image in images)
                    {
                        var imagelink = new LinkedResource(image.Stream, image.ContentType)
                        {
                            ContentId = image.Cid,
                            TransferEncoding = TransferEncoding.Base64
                        };
                        htmlView.LinkedResources.Add(imagelink);
                    }
                }
                mail.AlternateViews.Add(htmlView);
            }

            // Convert to raw format
            var mimeMessage = MimeMessage.CreateFromMailMessage(mail);
            using var memoryStream = new MemoryStream();
            await mimeMessage.WriteToAsync(memoryStream);
            var request = new SendRawEmailRequest { RawMessage = new() { Data = memoryStream } };

            _logger.LogInformation("Sending email to {To}", string.Join(", ", to));
            await _amazonSimpleEmailService.SendRawEmailAsync(request);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            return false;
        }
    }
}
