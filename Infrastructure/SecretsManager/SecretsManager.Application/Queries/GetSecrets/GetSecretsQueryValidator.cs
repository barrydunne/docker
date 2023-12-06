using FluentValidation;
using FluentValidation.Results;

namespace SecretsManager.Application.Queries.GetSecrets;

/// <summary>
/// Validation rules for <see cref="GetSecretsQuery"/>.
/// </summary>
internal class GetSecretsQueryValidator : AbstractValidator<GetSecretsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSecretsQueryValidator"/> class.
    /// </summary>
    public GetSecretsQueryValidator()
    {
        RuleFor(_ => _.Vault)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc/>
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetSecretsQuery> context, CancellationToken cancellation = default)
    {
        return (context.InstanceToValidate == null)
        ? Task.FromResult(new ValidationResult([new ValidationFailure(nameof(GetSecretsQuery), $"'{nameof(GetSecretsQuery)}' must not be null.")]))
        : base.ValidateAsync(context, cancellation);
    }
}
