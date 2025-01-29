using FluentValidation.TestHelper;
using Microservices.Shared.Events;
using Weather.Application.Commands.GenerateWeather;

namespace Weather.Application.Tests.Commands.GenerateWeather;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class GenerateWeatherCommandValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GenerateWeatherCommandValidatorTestsContext _context = new();

    [Test]
    public async Task GenerateWeatherCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GenerateWeatherCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<GenerateWeatherCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GenerateWeatherCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<GenerateWeatherCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GenerateWeatherCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<GenerateWeatherCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GenerateWeatherCommandValidator_fails_for_missing_coordinates()
    {
        var command = _fixture.Build<GenerateWeatherCommand>()
                              .With(_ => _.Coordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GenerateWeatherCommandValidator_returns_message_for_missing_coordinates()
    {
        var command = _fixture.Build<GenerateWeatherCommand>()
                              .With(_ => _.Coordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Coordinates) && _.ErrorMessage == "'Coordinates' must not be empty.");
        error.ShouldNotBeNull();
    }
}
