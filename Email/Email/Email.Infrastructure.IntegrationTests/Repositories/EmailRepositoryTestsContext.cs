using Email.Application.Models;
using Email.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Email.Infrastructure.IntegrationTests.Repositories;

internal class EmailRepositoryTestsContext : IDisposable
{
    private readonly Fixture _fixture;
    private readonly EmailRepositoryDbContext _db;
    private readonly string _database;

    private bool _disposedValue;

    internal EmailRepository Sut { get; }
    internal ConcurrentBag<SentEmail> SeedData { get; }

    public EmailRepositoryTestsContext()
    {
        /* These tests require a user and password to be created in the target MySQL. For example:
         *
         * CREATE USER 'integration.tests'@'%' IDENTIFIED BY 'password';
         * GRANT ALL PRIVILEGES ON *.* TO 'integration.tests'@'%' WITH GRANT OPTION;
         * FLUSH PRIVILEGES;
         */

        _fixture = new();
        _database = _fixture.Create<string>();
        var connectionString = $"Server=localhost;Port=17306;User Id=integration.tests;Password=password;Database={_database};";
        var options = new DbContextOptionsBuilder<EmailRepositoryDbContext>().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)).Options;
        _db = new EmailRepositoryDbContext(options);
        _db.Database.EnsureCreated();

        _disposedValue = false;

        SeedData = new();

        AddSeedData();

        Sut = new(_db);
    }

    private void AddSeedData()
    {
        for (var i = 0; i < 30; i++)
        {
            var sent = new SentEmail { RecipientEmail = "user@example.com", SentTime = DateTimeOffset.UtcNow.AddDays(i), JobId = _fixture.Create<Guid>() };
            SeedData.Add(sent);
            _db.SentEmails.Add(sent);
        }
        _db.SaveChanges();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _db.Database.EnsureDeleted();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
