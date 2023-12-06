using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Email.Application.Models;

/// <summary>
/// Contains details about an email that was sent.
/// </summary>
public class SentEmail
{
    /// <summary>
    /// Gets or sets the unique id for this entity.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address of the recipient.
    /// </summary>
    public string RecipientEmail { get; set; } = null!;

    /// <summary>
    /// Gets or sets the time the email was sent.
    /// </summary>
    public DateTimeOffset SentTime { get; set; }

    /// <summary>
    /// Gets or sets the id of the job that the email relates to.
    /// </summary>
    public Guid JobId { get; set; }
}
