using FluentValidation.TestHelper;
using Microservices.Shared.Events;
using State.Application.Commands.UpdateDirectionsResult;

namespace State.Application.Tests.Commands.UpdateDirectionsResult;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class UpdateDirectionsResultCommandValidatorTests
{
    private readonly StateFixture _fixture = new();
    private readonly UpdateDirectionsResultCommandValidatorTestsContext _context = new();

    [Test]
    public async Task UpdateDirectionsResultCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<UpdateDirectionsResultCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task UpdateDirectionsResultCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<UpdateDirectionsResultCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task UpdateDirectionsResultCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateDirectionsResultCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task UpdateDirectionsResultCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateDirectionsResultCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public async Task UpdateDirectionsResultCommandValidator_fails_for_missing_directions()
    {
        var command = _fixture.Build<UpdateDirectionsResultCommand>()
                              .With(_ => _.Directions, (Directions)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task UpdateDirectionsResultCommandValidator_returns_message_for_missing_directions()
    {
        var command = _fixture.Build<UpdateDirectionsResultCommand>()
                              .With(_ => _.Directions, (Directions)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Directions) && _.ErrorMessage == "'Directions' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }
}
