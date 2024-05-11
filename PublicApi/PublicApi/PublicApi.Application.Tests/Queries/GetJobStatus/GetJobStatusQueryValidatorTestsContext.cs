using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using PublicApi.Application.Queries.GetJobStatus;

namespace PublicApi.Application.Tests.Queries.GetJobStatus;

internal class GetJobStatusQueryValidatorTestsContext
{
    private readonly IGetJobStatusQueryHandlerMetrics _mockMetrics;

    internal GetJobStatusQueryValidator Sut { get; }

    public GetJobStatusQueryValidatorTestsContext()
    {
        _mockMetrics = Substitute.For<IGetJobStatusQueryHandlerMetrics>();
        Sut = new(_mockMetrics, new NullLogger<GetJobStatusQueryValidator>());
    }

    internal GetJobStatusQueryValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGuardTime(Arg.Any<double>());
        return this;
    }
}
