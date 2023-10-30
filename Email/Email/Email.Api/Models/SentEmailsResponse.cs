using Email.Repository.Models;

namespace Email.Api.Models
{
    /// <summary>
    /// The response to a search for emails.
    /// </summary>
    public class SentEmailsResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SentEmailsResponse"/> class.
        /// </summary>
        /// <param name="sentEmails">The email details to include in the response.</param>
        internal SentEmailsResponse(IEnumerable<SentEmail> sentEmails)
            => Emails = sentEmails.Select(_ => new EmailDetails(_.JobId, _.RecipientEmail, _.SentUtc)).ToArray();

        /// <summary>
        /// Gets the details of sent emails matching the request parameters.
        /// </summary>
        public EmailDetails[] Emails { get; }
    }
}
