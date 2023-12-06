using FluentValidation.TestHelper;
using Microservices.Shared.Events;
using State.Application.Commands.UpdateImagingResult;

namespace State.Application.Tests.Commands.UpdateImagingResult;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class UpdateImagingResultCommandValidatorTests
{
    private readonly StateFixture _fixture = new();
    private readonly UpdateImagingResultCommandValidatorTestsContext _context = new();

    [Test]
    public async Task UpdateImagingResultCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<UpdateImagingResultCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task UpdateImagingResultCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<UpdateImagingResultCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task UpdateImagingResultCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateImagingResultCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task UpdateImagingResultCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateImagingResultCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public async Task UpdateImagingResultCommandValidator_fails_for_missing_imaging()
    {
        var command = _fixture.Build<UpdateImagingResultCommand>()
                              .With(_ => _.Imaging, (ImagingResult)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task UpdateImagingResultCommandValidator_returns_message_for_missing_imaging()
    {
        var command = _fixture.Build<UpdateImagingResultCommand>()
                              .With(_ => _.Imaging, (ImagingResult)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Imaging) && _.ErrorMessage == "'Imaging' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }
}
