using FluentValidation.TestHelper;
using PublicApi.Application.Queries.GetJobStatus;

namespace PublicApi.Application.Tests.Queries.GetJobStatus;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetJobStatusQueryValidatorTests
{
    private readonly PublicApiFixture _fixture = new();
    private readonly GetJobStatusQueryValidatorTestsContext _context = new();

    [Test]
    public async Task GetJobStatusQueryValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<GetJobStatusQuery>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GetJobStatusQueryValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<GetJobStatusQuery>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task CreateJobCommandHandler_fails_for_missing_job_id()
    {
        var command = _fixture.Build<GetJobStatusQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<GetJobStatusQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }
}
