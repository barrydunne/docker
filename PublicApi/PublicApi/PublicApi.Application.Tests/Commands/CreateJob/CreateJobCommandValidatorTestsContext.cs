using Microsoft.Extensions.Logging;
using Moq;
using PublicApi.Application.Commands.CreateJob;

namespace PublicApi.Application.Tests.Commands.CreateJob;

internal class CreateJobCommandValidatorTestsContext
{
    private readonly Mock<ICreateJobCommandHandlerMetrics> _mockMetrics;

    internal CreateJobCommandValidator Sut { get; }

    public CreateJobCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<CreateJobCommandValidator>>().Object);
    }

    internal CreateJobCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}
