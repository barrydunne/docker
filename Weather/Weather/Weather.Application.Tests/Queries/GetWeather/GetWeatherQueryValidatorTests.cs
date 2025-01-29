using FluentValidation.TestHelper;
using Microservices.Shared.Events;
using Weather.Application.Queries.GetWeather;

namespace Weather.Application.Tests.Queries.GetWeather;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetWeatherQueryValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetWeatherQueryValidatorTestsContext _context = new();

    [Test]
    public async Task GetWeatherQueryValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<GetWeatherQuery>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GetWeatherQueryValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<GetWeatherQuery>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GetWeatherQueryValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<GetWeatherQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetWeatherQueryValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<GetWeatherQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetWeatherQueryValidator_fails_for_missing_coordinates()
    {
        var command = _fixture.Build<GetWeatherQuery>()
                              .With(_ => _.Coordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetWeatherQueryValidator_returns_message_for_missing_coordinates()
    {
        var command = _fixture.Build<GetWeatherQuery>()
                              .With(_ => _.Coordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Coordinates) && _.ErrorMessage == "'Coordinates' must not be empty.");
        error.ShouldNotBeNull();
    }
}
