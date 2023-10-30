namespace Microservices.Shared.CloudEmail
{
    /// <summary>
    /// Provides the ability to send emails.
    /// </summary>
    public interface ICloudEmail
    {
        /// <summary>
        /// Send a HTML email to one or more recipients.
        /// </summary>
        /// <param name="subject">The email subject.</param>
        /// <param name="htmlBody">The email body in HTML format.</param>
        /// <param name="to">The email recipient addresses.</param>
        /// <returns>Whether the email was sent.</returns>
        Task<bool> SendEmailAsync(string subject, string htmlBody, params string[] to);

        /// <summary>
        /// Send an email to one or more recipients.
        /// </summary>
        /// <param name="subject">The email subject.</param>
        /// <param name="htmlBody">The email body in HTML format.</param>
        /// <param name="plainBody">The email body in plain text format.</param>
        /// <param name="to">The email recipient addresses.</param>
        /// <param name="cc">The email cc recipient addresses.</param>
        /// <param name="bcc">The email bcc recipient addresses.</param>
        /// <param name="images">Any embedded images that have cid: links in the HTML body.</param>
        /// <returns>Whether the email was sent.</returns>
        Task<bool> SendEmailAsync(string subject, string? htmlBody, string? plainBody, string[] to, string[]? cc, string[]? bcc, params (string Cid, Stream Stream, string ContentType)[] images);
    }
}
