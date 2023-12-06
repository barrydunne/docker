using Email.Application.Models;
using Email.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Email.Infrastructure.Repositories;

/// <inheritdoc/>
public class EmailRepository : IEmailRepository
{
    private readonly EmailRepositoryDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailRepository"/> class.
    /// </summary>
    /// <param name="context">The DbContext used for the repository.</param>
    public EmailRepository(EmailRepositoryDbContext context) => _context = context;

    /// <inheritdoc/>
    public async Task InsertAsync(SentEmail sentEmail, CancellationToken cancellationToken = default)
    {
        await _context.SentEmails.AddAsync(sentEmail, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<SentEmail>> GetEmailsSentToRecipientAsync(string recipientEmail, int skip, int take, CancellationToken cancellationToken = default)
        => await _context.SentEmails.Where(_ => _.RecipientEmail.ToLower() == recipientEmail.ToLower()).OrderBy(_ => _.SentTime).Skip(skip).Take(take).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<List<SentEmail>> GetEmailsSentBetweenTimesAsync(DateTimeOffset from, DateTimeOffset to, int skip, int take, CancellationToken cancellationToken = default)
        => await _context.SentEmails.Where(_ => _.SentTime >= from && _.SentTime <= to).OrderBy(_ => _.SentTime).Skip(skip).Take(take).ToListAsync(cancellationToken);
}
