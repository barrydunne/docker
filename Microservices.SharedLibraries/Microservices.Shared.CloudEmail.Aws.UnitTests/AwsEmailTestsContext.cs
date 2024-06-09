using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using MimeKit;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Collections.Concurrent;
using System.Net.Mail;

namespace Microservices.Shared.CloudEmail.Aws.UnitTests;

internal class AwsEmailTestsContext
{
    private readonly Fixture _fixture;
    private readonly AwsEmailOptions _options;
    private readonly IOptions<AwsEmailOptions> _mockOptions;
    private readonly IAmazonSimpleEmailService _mockAmazonSimpleEmailServiceV2;
    private readonly MockLogger<AwsEmail> _mockLogger;
    private readonly ConcurrentBag<MimeMessage> _mails;

    internal AwsEmail Sut { get; }

    internal AwsEmailTestsContext()
    {
        _fixture = new();
        _options = _fixture.Build<AwsEmailOptions>().With(_ => _.From, _fixture.Create<MailAddress>().Address).Create();

        _mockOptions = Substitute.For<IOptions<AwsEmailOptions>>();
        _mockOptions
            .Value
            .Returns(callInfo => _options);
        
        _mails = new();
        _mockLogger = new();

        _mockAmazonSimpleEmailServiceV2 = Substitute.For<IAmazonSimpleEmailService>();
        _mockAmazonSimpleEmailServiceV2
            .When(_ => _.SendRawEmailAsync(Arg.Any<SendRawEmailRequest>()))
            .Do(callInfo =>
            {
                var request = callInfo.ArgAt<SendRawEmailRequest>(0);
                request.RawMessage.Data.Seek(0, SeekOrigin.Begin);
                var mime = MimeMessage.Load(request.RawMessage.Data);
                _mails.Add(mime);
            });

        Sut = new(_mockOptions, _mockAmazonSimpleEmailServiceV2, _mockLogger);
    }

    internal AwsEmailTestsContext WithSendException()
    {
        _mockAmazonSimpleEmailServiceV2
            .SendRawEmailAsync(Arg.Any<SendRawEmailRequest>())
            .Throws<InvalidOperationException>();
        return this;
    }

    internal void AssertMessageSent(Func<MimeMessage, bool> assertion)
    {
        var message = _mails.FirstOrDefault();
        Assert.That(message, Is.Not.Null, "No message sent");
        Assert.That(assertion.Invoke(message!), Is.True, "Assertion failed");
    }
}
