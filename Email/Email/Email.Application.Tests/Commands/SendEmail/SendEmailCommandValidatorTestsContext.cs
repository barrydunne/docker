using Email.Application.Commands.SendEmail;
using Microsoft.Extensions.Logging;
using Moq;

namespace Email.Application.Tests.Commands.SendEmail;

internal class SendEmailCommandValidatorTestsContext
{
    private readonly Mock<ISendEmailCommandHandlerMetrics> _mockMetrics;

    internal SendEmailCommandValidator Sut { get; }

    public SendEmailCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<SendEmailCommandValidator>>().Object);
    }

    internal SendEmailCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
