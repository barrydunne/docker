using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Collections.Concurrent;
using System.Net.Mail;

namespace Microservices.Shared.CloudEmail.Smtp.UnitTests;

internal class SmtpEmailTestsContext
{
    private readonly Fixture _fixture;
    private readonly SmtpEmailOptions _options;
    private readonly IOptions<SmtpEmailOptions> _mockOptions;
    private readonly ISmtpClient _mockSmtpClient;
    private readonly MockLogger<SmtpEmail> _mockLogger;
    private readonly ConcurrentBag<MailMessage> _mails;

    internal SmtpEmail Sut { get; }

    internal SmtpEmailTestsContext()
    {
        _fixture = new();
        _options = _fixture.Build<SmtpEmailOptions>().With(_ => _.From, _fixture.Create<MailAddress>().Address).Create();

        _mockOptions = Substitute.For<IOptions<SmtpEmailOptions>>();
        _mockOptions
            .Value
            .Returns(callInfo => _options);
        
        _mails = new();
        _mockLogger = new();

        _mockSmtpClient = Substitute.For<ISmtpClient>();
        _mockSmtpClient
            .When(_ => _.SendMailAsync(Arg.Any<MailMessage>()))
            .Do(callInfo => _mails.Add(callInfo.ArgAt<MailMessage>(0)));

        Sut = new(_mockOptions, _mockSmtpClient, _mockLogger);
    }

    internal SmtpEmailTestsContext WithSendException()
    {
        _mockSmtpClient
            .SendMailAsync(Arg.Any<MailMessage>())
            .Throws<InvalidOperationException>();
        return this;
    }

    internal SmtpEmailTestsContext AssertHostSet()
    {
        _mockSmtpClient.Received(1).Host = _options.Host;
        return this;
    }

    internal SmtpEmailTestsContext AssertPortSet()
    {
        _mockSmtpClient.Received(1).Port = _options.Port;
        return this;
    }

    internal void AssertMessageSent(Func<MailMessage, bool> assertion)
    {
        var message = _mails.FirstOrDefault();
        Assert.That(message, Is.Not.Null, "No message sent");
        Assert.That(assertion.Invoke(message!), Is.True, "Assertion failed");
    }
}
