using Email.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Email.Repository.UnitTests
{
    internal class EmailRepositoryTestsContext
    {
        private readonly Fixture _fixture;
        private readonly EmailRepositoryDbContext _db;

        internal EmailRepository Sut { get; }
        internal ConcurrentBag<SentEmail> SeedData { get; }

        public EmailRepositoryTestsContext()
        {
            _fixture = new();

            var options = new DbContextOptionsBuilder<EmailRepositoryDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _db = new EmailRepositoryDbContext(options);
            SeedData = new();

            AddSeedData();

            Sut = new(_db);
        }

        private void AddSeedData()
        {
            for (var i = 0; i < 30; i++)
            {
                var sent = new SentEmail { RecipientEmail = "user@example.com", SentUtc = DateTime.UtcNow.AddDays(i), JobId = _fixture.Create<Guid>() };
                SeedData.Add(sent);
                _db.SentEmails.Add(sent);
            }
            _db.SaveChanges();
        }
    }
}
