using Microsoft.Extensions.Logging.Abstractions;
using PublicApi.Application.Commands.UpdateStatus;

namespace PublicApi.Application.Tests.Commands.UpdateStatus;

internal class UpdateStatusCommandValidatorTestsContext
{
    private readonly IUpdateStatusCommandHandlerMetrics _mockMetrics;

    internal UpdateStatusCommandValidator Sut { get; }

    public UpdateStatusCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IUpdateStatusCommandHandlerMetrics>();
        Sut = new(_mockMetrics, new NullLogger<UpdateStatusCommandValidator>());
    }

    internal UpdateStatusCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
