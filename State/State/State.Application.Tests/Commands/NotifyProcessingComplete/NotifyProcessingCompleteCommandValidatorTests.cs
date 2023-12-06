using FluentValidation.TestHelper;
using State.Application.Commands.NotifyProcessingComplete;

namespace State.Application.Tests.Commands.NotifyProcessingComplete;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class NotifyProcessingCompleteCommandValidatorTests
{
    private readonly StateFixture _fixture = new();
    private readonly NotifyProcessingCompleteCommandValidatorTestsContext _context = new();

    [Test]
    public async Task NotifyProcessingCompleteCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<NotifyProcessingCompleteCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task NotifyProcessingCompleteCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<NotifyProcessingCompleteCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task NotifyProcessingCompleteCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<NotifyProcessingCompleteCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task NotifyProcessingCompleteCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<NotifyProcessingCompleteCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }
}
