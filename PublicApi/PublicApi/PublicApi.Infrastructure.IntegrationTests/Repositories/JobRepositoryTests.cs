using Microservices.Shared.Events;
using PublicApi.Application.Models;
using System.Text.Json;

namespace PublicApi.Infrastructure.IntegrationTests.Repositories;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "JobRepository")]
public class JobRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly JobRepositoryTestsContext _context = new();

    public JobRepositoryTests() => _fixture.Customize<Job>(_ => _.With(_ => _.CreatedUtc, UtcNowWithLowPrecision()));

    private DateTime UtcNowWithLowPrecision()
    {
        // This is to avoid precision differences when saving/loading from db
        var now = DateTime.UtcNow;
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
    }

    [TearDown]
    public void TearDown() => _context.DeleteDatabase();

    [Test]
    public async Task JobRepository_InsertAsync_inserts_job()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        _context.AssertJobSaved(job);
    }

    [Test]
    public async Task JobRepository_GetJobByIdAsync_returns_job_if_exists()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(JsonSerializer.Serialize(retrieved), Is.EqualTo(JsonSerializer.Serialize(job)));
    }

    [Test]
    public async Task JobRepository_GetJobByIdAsync_returns_null_if_unknown()
    {
        var job = _fixture.Create<Job>();
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(retrieved, Is.Null);
    }

    [Test]
    public async Task JobRepository_GetJobIdByIdempotencyKeyAsync_returns_job_if_exists()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var retrieved = await _context.Sut.GetJobIdByIdempotencyKeyAsync(job.IdempotencyKey);
        Assert.That(retrieved, Is.EqualTo(job.JobId));
    }

    [Test]
    public async Task JobRepository_GetJobIdByIdempotencyKeyAsync_returns_null_if_unknown()
    {
        var job = _fixture.Create<Job>();
        var retrieved = await _context.Sut.GetJobIdByIdempotencyKeyAsync(job.IdempotencyKey);
        Assert.That(retrieved, Is.Null);
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_status()
    {
        var job = _fixture.Create<Job>();

        // Ensure updating to a different status
        var status = _fixture.Create<JobStatus>();
        while (status == job.Status)
            job.Status = _fixture.Create<JobStatus>();

        await _context.Sut.InsertAsync(job);
        await _context.Sut.UpdateJobStatusAsync(job.JobId, status, null);
        _context.AssertJobStatus(job.JobId, status);
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_additional_information()
    {
        var job = _fixture.Create<Job>();
        var additionalInformation = _fixture.Create<string>();
        await _context.Sut.InsertAsync(job);
        await _context.Sut.UpdateJobStatusAsync(job.JobId, job.Status, additionalInformation);
        _context.AssertJobAdditionalInformation(job.JobId, additionalInformation);
    }

    [Test]
    public async Task JobRepository_InsertAsync_creates_jobid_index()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        _context.AssertIndexCreated(nameof(Job.JobId));
    }

    [Test]
    public async Task JobRepository_InsertAsync_creates_idempotency_key_index()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        _context.AssertIndexCreated(nameof(Job.IdempotencyKey));
    }

    [Test]
    public async Task JobRepository_InsertAsync_silently_ignores_existing_index()
    {
        var job = _fixture.Create<Job>();
        _context.WithExistingIndex(nameof(Job.JobId), true);
        await _context.Sut.InsertAsync(job);
        _context.AssertIndexCreated(nameof(Job.IdempotencyKey));
    }
}
