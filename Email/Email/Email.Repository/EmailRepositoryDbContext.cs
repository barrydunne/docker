using Email.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Email.Repository
{
    /// <summary>
    /// The <see cref="DbContext"/> for the email repository.
    /// </summary>
    public class EmailRepositoryDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailRepositoryDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public EmailRepositoryDbContext(DbContextOptions<EmailRepositoryDbContext> options) : base(options) { }

        /// <summary>
        /// Gets the set of <see cref="SentEmail"/> records.
        /// </summary>
        public DbSet<SentEmail> SentEmails => Set<SentEmail>();

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SentEmail>()
                .ToTable("sentemail")
                .Property(_ => _.SentUtc)
                .HasConversion(_ => _.ToUniversalTime(), _ => DateTime.SpecifyKind(_, DateTimeKind.Utc));

            modelBuilder.Entity<SentEmail>().HasIndex(_ => _.RecipientEmail);
            modelBuilder.Entity<SentEmail>().HasIndex(_ => _.SentUtc);

            base.OnModelCreating(modelBuilder);
        }
    }
}
