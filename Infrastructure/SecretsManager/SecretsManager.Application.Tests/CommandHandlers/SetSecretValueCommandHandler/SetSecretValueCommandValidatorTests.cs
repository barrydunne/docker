using FluentValidation.TestHelper;
using SecretsManager.Application.Commands.SetSecretValue;

namespace SecretsManager.Application.Tests.CommandHandlers.SetSecretValueCommandHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class SetSecretValueCommandValidatorTests
{
    private readonly Fixture _fixture;
    private readonly SetSecretValueCommandValidator _sut;

    public SetSecretValueCommandValidatorTests()
    {
        _fixture = new();
        _sut = new();
    }

    [Test]
    public async Task SetSecretValueCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<SetSecretValueCommand>();
        var result = await _sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task SetSecretValueCommandValidator_fails_for_missing_vault()
    {
        var command = _fixture.Build<SetSecretValueCommand>()
                              .With(_ => _.Vault, string.Empty)
                              .Create();
        var result = await _sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SetSecretValueCommandValidator_returns_message_for_missing_vault()
    {
        var command = _fixture.Build<SetSecretValueCommand>()
                              .With(_ => _.Vault, string.Empty)
                              .Create();
        var result = await _sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => (_.PropertyName == nameof(command.Vault)) && (_.ErrorMessage == "'Vault' must not be empty."));
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SetSecretValueCommandValidator_fails_for_missing_secret()
    {
        var command = _fixture.Build<SetSecretValueCommand>()
                              .With(_ => _.Secret, string.Empty)
                              .Create();
        var result = await _sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SetSecretValueCommandValidator_returns_message_for_missing_secret()
    {
        var command = _fixture.Build<SetSecretValueCommand>()
                              .With(_ => _.Secret, string.Empty)
                              .Create();
        var result = await _sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => (_.PropertyName == nameof(command.Secret)) && (_.ErrorMessage == "'Secret' must not be empty."));
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SetSecretValueCommandValidator_fails_for_null_command()
    {
        SetSecretValueCommand command = null!;
        var result = await _sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SetSecretValueCommandValidator_returns_message_for_null_command()
    {
        SetSecretValueCommand command = null!;
        var result = await _sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => (_.PropertyName == nameof(SetSecretValueCommand)) && (_.ErrorMessage == "'SetSecretValueCommand' must not be null."));
        error.ShouldNotBeNull();
    }
}
