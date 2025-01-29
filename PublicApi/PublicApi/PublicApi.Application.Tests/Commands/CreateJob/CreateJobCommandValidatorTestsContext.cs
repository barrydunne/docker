using Microsoft.Extensions.Logging.Abstractions;
using PublicApi.Application.Commands.CreateJob;

namespace PublicApi.Application.Tests.Commands.CreateJob;

internal class CreateJobCommandValidatorTestsContext
{
    private readonly ICreateJobCommandHandlerMetrics _mockMetrics;

    internal CreateJobCommandValidator Sut { get; }

    public CreateJobCommandValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<ICreateJobCommandHandlerMetrics>();
        Sut = new(_mockMetrics, new NullLogger<CreateJobCommandValidator>());
    }

    internal CreateJobCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
