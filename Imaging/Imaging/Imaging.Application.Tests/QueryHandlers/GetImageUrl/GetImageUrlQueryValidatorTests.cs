using FluentValidation.TestHelper;
using Imaging.Application.Queries.GetImageUrl;
using Microservices.Shared.Events;

namespace Imaging.Application.Tests.QueryHandlers.GetImageUrl;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetImageUrlQueryValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetImageUrlQueryValidatorTestsContext _context = new();

    [Test]
    public async Task GetImageUrlQueryValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<GetImageUrlQuery>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GetImageUrlQueryValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<GetImageUrlQuery>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GetImageUrlQueryValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<GetImageUrlQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetImageUrlQueryValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<GetImageUrlQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetImageUrlQueryValidator_fails_for_missing_address()
    {
        var command = _fixture.Build<GetImageUrlQuery>()
                              .With(_ => _.Address, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetImageUrlQueryValidator_returns_message_for_missing_address()
    {
        var command = _fixture.Build<GetImageUrlQuery>()
                              .With(_ => _.Address, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Address) && _.ErrorMessage == "'Address' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetImageUrlQueryValidator_fails_for_missing_coordinates()
    {
        var command = _fixture.Build<GetImageUrlQuery>()
                              .With(_ => _.Coordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetImageUrlQueryValidator_returns_message_for_missing_coordinates()
    {
        var command = _fixture.Build<GetImageUrlQuery>()
                              .With(_ => _.Coordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Coordinates) && _.ErrorMessage == "'Coordinates' must not be empty.");
        error.ShouldNotBeNull();
    }
}
