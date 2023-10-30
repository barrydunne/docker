namespace Email.Api.Models
{
    /// <summary>
    /// Gets all emails sent to a specific recipient.
    /// </summary>
    /// <param name="RecipientEmail">The recipient email address to search for.</param>
    /// <param name="PageSize">The page number of results to return. Starting with 1.</param>
    /// <param name="PageNumber">The number of results to return per page.</param>
    public record GetEmailsSentToRecipientRequest(string RecipientEmail, int PageSize, int PageNumber);
}
