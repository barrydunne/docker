using FluentValidation.TestHelper;
using SecretsManager.Application.Queries.GetSecretValue;

namespace SecretsManager.Application.Tests.QueryHandlers.GetSecretValueQueryHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetSecretValueQueryValidatorTests
{
    private readonly Fixture _fixture;
    private readonly GetSecretValueQueryValidator _sut;

    public GetSecretValueQueryValidatorTests()
    {
        _fixture = new();
        _sut = new();
    }

    [Test]
    public async Task GetSecretValueQueryValidator_succeeds_for_valid_instance()
    {
        var query = _fixture.Create<GetSecretValueQuery>();
        var result = await _sut.TestValidateAsync(query);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task GetSecretValueQueryValidator_fails_for_missing_vault()
    {
        var query = _fixture.Build<GetSecretValueQuery>()
                            .With(_ => _.Vault, string.Empty)
                            .Create();
        var result = await _sut.TestValidateAsync(query);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task GetSecretValueQueryValidator_returns_message_for_missing_vault()
    {
        var query = _fixture.Build<GetSecretValueQuery>()
                            .With(_ => _.Vault, string.Empty)
                            .Create();
        var result = await _sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.Vault) && _.ErrorMessage == "'Vault' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public async Task GetSecretValueQueryValidator_fails_for_missing_secret()
    {
        var query = _fixture.Build<GetSecretValueQuery>()
                            .With(_ => _.Secret, string.Empty)
                            .Create();
        var result = await _sut.TestValidateAsync(query);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task GetSecretValueQueryValidator_returns_message_for_missing_secret()
    {
        var query = _fixture.Build<GetSecretValueQuery>()
                            .With(_ => _.Secret, string.Empty)
                            .Create();
        var result = await _sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(query.Secret) && _.ErrorMessage == "'Secret' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public async Task GetSecretValueQueryValidator_fails_for_null_query()
    {
        GetSecretValueQuery query = null!;
        var result = await _sut.TestValidateAsync(query);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task GetSecretValueQueryValidator_returns_message_for_null_query()
    {
        GetSecretValueQuery query = null!;
        var result = await _sut.TestValidateAsync(query);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(GetSecretValueQuery) && _.ErrorMessage == "'GetSecretValueQuery' must not be null.");
        Assert.That(error, Is.Not.Null);
    }
}
