using Email.Application.Queries.GetEmailsSentToRecipient;
using FluentValidation.TestHelper;
using System.Net.Mail;

namespace Email.Application.Tests.Queries.GetEmailsSentToRecipient;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetEmailsSentToRecipientQueryValidatorTests
{
    private readonly EmailFixture _fixture = new();
    private readonly GetEmailsSentToRecipientQueryValidatorTestsContext _context = new();

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_metrics_records_guard_time()
    {
        var query = CreateGetEmailsSentToRecipientQuery();
        await _context.Sut.TestValidateAsync(query);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_succeeds_for_valid_instance()
    {
        var query = CreateGetEmailsSentToRecipientQuery();
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_fails_for_missing_email()
    {
        var query = CreateGetEmailsSentToRecipientQuery(email: string.Empty);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_returns_message_for_missing_email()
    {
        var query = CreateGetEmailsSentToRecipientQuery(email: string.Empty);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.Email) && _.ErrorMessage == "'Email' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_fails_for_invalid_email()
    {
        var query = CreateGetEmailsSentToRecipientQuery(email: "invalid");
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_returns_message_for_invalid_email()
    {
        var query = CreateGetEmailsSentToRecipientQuery(email: "invalid");
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.Email) && _.ErrorMessage == "'Email' is not a valid email address.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_fails_for_minimum_page_size()
    {
        var query = CreateGetEmailsSentToRecipientQuery(pageSize: int.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_returns_message_for_minimum_page_size()
    {
        var query = CreateGetEmailsSentToRecipientQuery(pageSize: int.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.PageSize) && _.ErrorMessage == "'Page Size' must be greater than or equal to '1'.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_fails_for_maximum_page_size()
    {
        var query = CreateGetEmailsSentToRecipientQuery(pageSize: int.MaxValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_returns_message_for_maximum_page_size()
    {
        var query = CreateGetEmailsSentToRecipientQuery(pageSize: int.MaxValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.PageSize) && _.ErrorMessage == "'Page Size' must be less than or equal to '500'.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_fails_for_minimum_page_number()
    {
        var query = CreateGetEmailsSentToRecipientQuery(pageNumber: int.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetEmailsSentToRecipientQueryValidator_returns_message_for_minimum_page_number()
    {
        var query = CreateGetEmailsSentToRecipientQuery(pageNumber: int.MinValue);
        var result = await _context.Sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.PageNumber) && _.ErrorMessage == "'Page Number' must be greater than or equal to '1'.");
        error.ShouldNotBeNull();
    }

    private GetEmailsSentToRecipientQuery CreateGetEmailsSentToRecipientQuery(string? email = null, int? pageSize = null, int? pageNumber = null)
        => _fixture.Build<GetEmailsSentToRecipientQuery>()
                   .With(_ => _.Email, email ?? _fixture.Create<MailAddress>().Address)
                   .With(_ => _.PageSize, pageSize ?? 50)
                   .With(_ => _.PageNumber, pageNumber ?? 1)
                   .Create();
}
