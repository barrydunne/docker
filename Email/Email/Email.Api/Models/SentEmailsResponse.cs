namespace Email.Api.Models;

/// <summary>
/// The response to a search for emails.
/// </summary>
/// <param name="Emails">The details of sent emails matching the request parameters.</param>
public record SentEmailsResponse(EmailDetails[] Emails);
