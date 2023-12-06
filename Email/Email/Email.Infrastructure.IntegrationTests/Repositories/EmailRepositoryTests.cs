﻿using Email.Application.Models;

namespace Email.Infrastructure.IntegrationTests.Repositories;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "EmailRepository")]
internal class EmailRepositoryTests : IDisposable
{
    private readonly Fixture _fixture = new();
    private readonly EmailRepositoryTestsContext _context = new();
    private bool _disposedValue = false;

    [Test]
    public async Task InsertSentEmailAsync_saves_entity_with_generated_id()
    {
        var entity = _fixture.Build<SentEmail>().Without(_ => _.Id).Create();
        await _context.Sut.InsertAsync(entity);
        var saved = await _context.Sut.GetEmailsSentToRecipientAsync(entity.RecipientEmail, 0, 1);
        Assert.That(saved.FirstOrDefault(_ => _.JobId == entity.JobId)?.Id, Is.Not.Null);
    }

    [Test]
    public async Task GetEmailsSentToRecipientAsync_gets_correct_page_of_data()
    {
        var pageSize = 8;
        var pageNumber = 3;
        var skip = (pageNumber - 1) * pageSize;
        var take = pageSize;
        var expected = _context.SeedData.OrderBy(_ => _.SentTime).Skip(skip).Take(take);
        var actual = await _context.Sut.GetEmailsSentToRecipientAsync(_context.SeedData.First().RecipientEmail, skip, take);
        Assert.That(string.Join(", ", actual.Select(_ => _.JobId)), Is.EqualTo(string.Join(", ", expected.Select(_ => _.JobId))));
    }

    [Test]
    public async Task GetEmailsSentBetweenTimesAsync_gets_correct_page_of_data()
    {
        var pageSize = 4;
        var pageNumber = 3;
        var skip = (pageNumber - 1) * pageSize;
        var take = pageSize;
        var start = new DateTimeOffset(DateTime.Today.AddDays(3));
        var end = new DateTimeOffset(DateTime.Today.AddDays(50));
        var expected = _context.SeedData.Where(_ => _.SentTime >= start && _.SentTime <= end).OrderBy(_ => _.SentTime).Skip(skip).Take(take);
        var actual = await _context.Sut.GetEmailsSentBetweenTimesAsync(start, end, skip, take);
        Assert.That(string.Join(", ", actual.Select(_ => _.JobId)), Is.EqualTo(string.Join(", ", expected.Select(_ => _.JobId))));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _context.Dispose();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
