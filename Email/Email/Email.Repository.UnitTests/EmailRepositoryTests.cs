using Email.Repository.Models;

namespace Email.Repository.UnitTests
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "EmailRepository")]
    internal class EmailRepositoryTests
    {
        private readonly Fixture _fixture = new();
        private readonly EmailRepositoryTestsContext _context = new();

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
            var expected = _context.SeedData.OrderBy(_ => _.SentUtc).Skip(skip).Take(take);
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
            var start = DateTime.Today.AddDays(3);
            var end = DateTime.Today.AddDays(50);
            var expected = _context.SeedData.Where(_ => (_.SentUtc >= start) && (_.SentUtc <= end)).OrderBy(_ => _.SentUtc).Skip(skip).Take(take);
            var actual = await _context.Sut.GetEmailsSentBetweenTimesAsync(start, end, skip, take);
            Assert.That(string.Join(", ", actual.Select(_ => _.JobId)), Is.EqualTo(string.Join(", ", expected.Select(_ => _.JobId))));
        }
    }
}
