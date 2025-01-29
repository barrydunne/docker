using FluentValidation.TestHelper;
using SecretsManager.Application.Queries.GetSecrets;

namespace SecretsManager.Application.Tests.QueryHandlers.GetSecretsQueryHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetSecretsQueryValidatorTests
{
    private readonly Fixture _fixture;
    private readonly GetSecretsQueryValidator _sut;

    public GetSecretsQueryValidatorTests()
    {
        _fixture = new();
        _sut = new();
    }

    [Test]
    public async Task GetSecretsQueryValidator_succeeds_for_valid_instance()
    {
        var query = _fixture.Create<GetSecretsQuery>();
        var result = await _sut.TestValidateAsync(query);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GetSecretsQueryValidator_fails_for_missing_vault()
    {
        var query = _fixture.Build<GetSecretsQuery>()
                            .With(_ => _.Vault, string.Empty)
                            .Create();
        var result = await _sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetSecretsQueryValidator_returns_message_for_missing_vault()
    {
        var query = _fixture.Build<GetSecretsQuery>()
                            .With(_ => _.Vault, string.Empty)
                            .Create();
        var result = await _sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => (_.PropertyName == nameof(query.Vault)) && (_.ErrorMessage == "'Vault' must not be empty."));
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetSecretsQueryValidator_fails_for_null_query()
    {
        GetSecretsQuery query = null!;
        var result = await _sut.TestValidateAsync(query);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetSecretsQueryValidator_returns_message_for_null_query()
    {
        GetSecretsQuery query = null!;
        var result = await _sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => (_.PropertyName == nameof(GetSecretsQuery)) && (_.ErrorMessage == "'GetSecretsQuery' must not be null."));
        error.ShouldNotBeNull();
    }
}
