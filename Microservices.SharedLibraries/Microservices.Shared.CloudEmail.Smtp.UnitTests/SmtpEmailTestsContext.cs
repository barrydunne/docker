using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Concurrent;
using System.Net.Mail;

namespace Microservices.Shared.CloudEmail.Smtp.UnitTests
{
    internal class SmtpEmailTestsContext
    {
        private readonly Fixture _fixture;
        private readonly SmtpEmailOptions _options;
        private readonly Mock<IOptions<SmtpEmailOptions>> _mockOptions;
        private readonly Mock<ISmtpClient> _mockSmtpClient;
        private readonly MockLogger<SmtpEmail> _mockLogger;
        private readonly ConcurrentBag<MailMessage> _mails;

        internal SmtpEmail Sut { get; }

        internal SmtpEmailTestsContext()
        {
            _fixture = new();
            _options = _fixture.Build<SmtpEmailOptions>().With(_ => _.From, _fixture.Create<MailAddress>().Address).Create();
            _mockOptions = new(MockBehavior.Strict);
            _mockOptions.Setup(_ => _.Value).Returns(_options);
            _mails = new();
            _mockLogger = new();

            _mockSmtpClient = new();
            _mockSmtpClient.Setup(_ => _.SendMailAsync(It.IsAny<MailMessage>()))
                .Callback((MailMessage message) => _mails.Add(message))
                .Returns(Task.CompletedTask);

            Sut = new(_mockOptions.Object, _mockSmtpClient.Object, _mockLogger.Object);
        }

        internal SmtpEmailTestsContext WithSendException()
        {
            _mockSmtpClient.Setup(_ => _.SendMailAsync(It.IsAny<MailMessage>())).Throws<InvalidOperationException>();
            return this;
        }

        internal SmtpEmailTestsContext AssertHostSet()
        {
            _mockSmtpClient.VerifySet(_ => _.Host = _options.Host);
            return this;
        }

        internal SmtpEmailTestsContext AssertPortSet()
        {
            _mockSmtpClient.VerifySet(_ => _.Port = _options.Port);
            return this;
        }

        internal void AssertMessageSent(Func<MailMessage, bool> assertion)
        {
            var message = _mails.FirstOrDefault();
            Assert.That(message, Is.Not.Null, "No message sent");
            Assert.That(assertion.Invoke(message), Is.True, "Assertion failed");
        }
    }
}
