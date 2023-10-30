namespace Email.Api.Models
{
    /// <summary>
    /// Details of a sent email.
    /// </summary>
    /// <param name="JobId">The id of the job that the email relates to.</param>
    /// <param name="RecipientEmail">The email address of the recipient.</param>
    /// <param name="SentUtc">The time the email was sent.</param>
    public record EmailDetails(Guid JobId, string RecipientEmail, DateTime SentUtc);
}
