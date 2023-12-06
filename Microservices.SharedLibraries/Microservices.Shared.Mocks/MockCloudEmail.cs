using Microservices.Shared.CloudEmail;
using Moq;
using System.Collections.Concurrent;

namespace Microservices.Shared.Mocks;

public class MockCloudEmail : Mock<ICloudEmail>
{
    public ConcurrentBag<(string Subject, string? HtmlBody, string? PlainBody, string[] To, string[]? Cc, string[]? Bcc, (string Cid, Stream Stream, string ContentType)[] Images)> Emails { get; }

    public MockCloudEmail() : base(MockBehavior.Strict)
    {
        Emails = new();

        Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .Callback((string subject, string htmlBody, string[] to) => SendEmail(subject, htmlBody, null, to, null, null, Array.Empty<(string Cid, Stream Stream, string ContentType)>()))
            .ReturnsAsync(() => true);

        Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string[]>(), It.IsAny<string[]?>(), It.IsAny<string[]?>(), It.IsAny<(string Cid, Stream Stream, string ContentType)[]>()))
            .Callback((string subject, string? htmlBody, string? plainBody, string[] to, string[]? cc, string[]? bcc, (string Cid, Stream Stream, string ContentType)[] images) => SendEmail(subject, htmlBody, plainBody, to, cc, bcc, images))
            .ReturnsAsync(() => true);
    }

    private void SendEmail(string subject, string? htmlBody, string? plainBody, string[] to, string[]? cc, string[]? bcc, params (string Cid, Stream Stream, string ContentType)[] images)
        => Emails.Add((subject, htmlBody, plainBody, to, cc, bcc, images));
}
