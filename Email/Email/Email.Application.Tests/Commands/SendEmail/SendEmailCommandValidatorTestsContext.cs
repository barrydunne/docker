using Email.Application.Commands.SendEmail;
using Microservices.Shared.Mocks;
using NSubstitute;

namespace Email.Application.Tests.Commands.SendEmail;

internal class SendEmailCommandValidatorTestsContext
{
    private readonly ISendEmailCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<SendEmailCommandValidator> _mockLogger;

    internal SendEmailCommandValidator Sut { get; }

    public SendEmailCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<ISendEmailCommandHandlerMetrics>();
        _mockLogger = new();
        Sut = new(_mockMetrics, _mockLogger);
    }

    internal SendEmailCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
