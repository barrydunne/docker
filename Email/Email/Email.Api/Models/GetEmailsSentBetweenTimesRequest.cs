namespace Email.Api.Models
{
    /// <summary>
    /// Gets all emails sent between specific times.
    /// </summary>
    /// <param name="FromUnixSeconds">The earliest sent email to search for.</param>
    /// <param name="ToUnixSeconds">The latest sent email to search for.</param>
    /// <param name="PageSize">The page number of results to return. Starting with 1.</param>
    /// <param name="PageNumber">The number of results to return per page.</param>
    public record GetEmailsSentBetweenTimesRequest(long FromUnixSeconds, long ToUnixSeconds, int PageSize, int PageNumber);
}
