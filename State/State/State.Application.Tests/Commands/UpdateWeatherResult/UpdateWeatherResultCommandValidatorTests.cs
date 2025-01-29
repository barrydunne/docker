using FluentValidation.TestHelper;
using Microservices.Shared.Events;
using State.Application.Commands.UpdateWeatherResult;

namespace State.Application.Tests.Commands.UpdateWeatherResult;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class UpdateWeatherResultCommandValidatorTests
{
    private readonly StateFixture _fixture = new();
    private readonly UpdateWeatherResultCommandValidatorTestsContext _context = new();

    [Test]
    public async Task UpdateWeatherResultCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<UpdateWeatherResultCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task UpdateWeatherResultCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<UpdateWeatherResultCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task UpdateWeatherResultCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateWeatherResultCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task UpdateWeatherResultCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateWeatherResultCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task UpdateWeatherResultCommandValidator_fails_for_missing_weather()
    {
        var command = _fixture.Build<UpdateWeatherResultCommand>()
                              .With(_ => _.Weather, (WeatherForecast)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task UpdateWeatherResultCommandValidator_returns_message_for_missing_weather()
    {
        var command = _fixture.Build<UpdateWeatherResultCommand>()
                              .With(_ => _.Weather, (WeatherForecast)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Weather) && _.ErrorMessage == "'Weather' must not be empty.");
        error.ShouldNotBeNull();
    }
}
