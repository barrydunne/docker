using Email.Application.Queries.GetEmailsSentBetweenTimes;
using FluentValidation.TestHelper;

namespace Email.Application.Tests.Queries.GetEmailsSentBetweenTimes;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetEmailsSentBetweenTimesQueryValidatorTests
{
    private readonly EmailFixture _fixture = new();
    private readonly GetEmailsSentBetweenTimesQueryValidatorTestsContext _context = new();

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_metrics_records_guard_time()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery();
        await _context.Sut.TestValidateAsync(query);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_succeeds_for_valid_instance()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery();
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_fails_for_early_from_time()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(fromTime: DateTimeOffset.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_returns_message_for_early_from_time()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(fromTime: DateTimeOffset.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.FromTime) && _.ErrorMessage.StartsWith("'From Time' must be greater than or equal to "));
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_fails_for_late_from_time()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(fromTime: DateTimeOffset.MaxValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_returns_message_for_late_from_time()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(fromTime: DateTimeOffset.MaxValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.FromTime) && _.ErrorMessage.StartsWith("'From Time' must be less than or equal to "));
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_fails_for_early_to_time()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(toTime: DateTimeOffset.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_returns_message_for_early_to_time()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(toTime: DateTimeOffset.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.ToTime) && _.ErrorMessage.StartsWith("'To Time' must be greater than or equal to "));
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_fails_for_late_to_time()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(toTime: DateTimeOffset.MaxValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_returns_message_for_late_to_time()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(toTime: DateTimeOffset.MaxValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.ToTime) && _.ErrorMessage.StartsWith("'To Time' must be less than or equal to "));
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_fails_for_minimum_page_size()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(pageSize: int.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_returns_message_for_minimum_page_size()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(pageSize: int.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.PageSize) && _.ErrorMessage == "'Page Size' must be greater than or equal to '1'.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_fails_for_maximum_page_size()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(pageSize: int.MaxValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_returns_message_for_maximum_page_size()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(pageSize: int.MaxValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.PageSize) && _.ErrorMessage == "'Page Size' must be less than or equal to '500'.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_fails_for_minimum_page_number()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(pageNumber: int.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesQueryValidator_returns_message_for_minimum_page_number()
    {
        var query = CreateGetEmailsSentBetweenTimesQuery(pageNumber: int.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.PageNumber) && _.ErrorMessage == "'Page Number' must be greater than or equal to '1'.");
        error.ShouldNotBeNull();
    }

    private GetEmailsSentBetweenTimesQuery CreateGetEmailsSentBetweenTimesQuery(DateTimeOffset? fromTime = null, DateTimeOffset? toTime = null, int? pageSize = null, int? pageNumber = null)
        => _fixture.Build<GetEmailsSentBetweenTimesQuery>()
                   .With(_ => _.FromTime, fromTime ?? DateTimeOffset.UtcNow)
                   .With(_ => _.ToTime, toTime ?? DateTimeOffset.UtcNow)
                   .With(_ => _.PageSize, pageSize ?? 50)
                   .With(_ => _.PageNumber, pageNumber ?? 1)
                   .Create();
}
