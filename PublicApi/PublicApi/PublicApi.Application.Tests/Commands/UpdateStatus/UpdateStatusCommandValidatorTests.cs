using FluentValidation.TestHelper;
using PublicApi.Application.Commands.UpdateStatus;

namespace PublicApi.Application.Tests.Commands.UpdateStatus;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class UpdateStatusCommandValidatorTests
{
    private readonly PublicApiFixture _fixture = new();
    private readonly UpdateStatusCommandValidatorTestsContext _context = new();

    [Test]
    public async Task UpdateStatusCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<UpdateStatusCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task UpdateStatusCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<UpdateStatusCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task CreateJobCommandHandler_fails_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateStatusCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task CreateJobCommandHandler_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateStatusCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }
}
