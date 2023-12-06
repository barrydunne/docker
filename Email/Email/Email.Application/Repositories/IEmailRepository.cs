using Email.Application.Models;

namespace Email.Application.Repositories;

/// <summary>
/// The repository for storing sent email details.
/// </summary>
public interface IEmailRepository
{
    /// <summary>
    /// Insert a new record into the repository.
    /// </summary>
    /// <param name="sentEmail">The record to insert.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task InsertAsync(SentEmail sentEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all emails sent to a specific recipient.
    /// </summary>
    /// <param name="recipientEmail">The recipient email address to search for.</param>
    /// <param name="skip">The number of records to skip. Used for pagination.</param>
    /// <param name="take">The maximum number of records to return. Used for pagination.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>All emails sent to the recipient.</returns>
    Task<List<SentEmail>> GetEmailsSentToRecipientAsync(string recipientEmail, int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all emails sent between specific times.
    /// </summary>
    /// <param name="from">The earliest sent email to search for.</param>
    /// <param name="to">The latest sent email to search for.</param>
    /// <param name="skip">The number of records to skip. Used for pagination.</param>
    /// <param name="take">The maximum number of records to return. Used for pagination.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>All emails sent between the specified times.</returns>
    Task<List<SentEmail>> GetEmailsSentBetweenTimesAsync(DateTimeOffset from, DateTimeOffset to, int skip, int take, CancellationToken cancellationToken = default);
}
