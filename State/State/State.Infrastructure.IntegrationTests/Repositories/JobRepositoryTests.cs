using Microservices.Shared.Events;
using State.Application.Models;
using System.Text.Json;

namespace State.Infrastructure.IntegrationTests.Repositories;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "JobRepository")]
public class JobRepositoryTests
{
    private readonly Fixture _fixture;
    private readonly JobRepositoryTestsContext _context;

    public JobRepositoryTests()
    {
        _fixture = new();
        _fixture.Customizations.Add(new JobRepositorySpecimenBuilder());
        _context = new();
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
    public async Task JobRepository_InsertAsync_creates_jobid_index()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        _context.AssertIndexCreated(nameof(Job.JobId));
    }

    [Test]
    public async Task JobRepository_InsertAsync_silently_ignores_existing_index()
    {
        var job = _fixture.Create<Job>();
        _context.WithExistingIndex(nameof(Job.JobId), true);
        await _context.Sut.InsertAsync(job);
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_null_if_job_unknown()
    {
        var job = _fixture.Create<Job>();
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        Assert.That(status, Is.Null);
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_null_if_directions_not_complete()
    {
        var job = _fixture.Create<Job>();
        job.DirectionsSuccessful = null;
        job.Directions = null;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        Assert.That(status, Is.Null);
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_null_if_weather_not_complete()
    {
        var job = _fixture.Create<Job>();
        job.WeatherSuccessful = null;
        job.WeatherForecast = null;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        Assert.That(status, Is.Null);
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_null_if_imaging_not_complete()
    {
        var job = _fixture.Create<Job>();
        job.ImagingSuccessful = null;
        job.ImagingResult = null;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        Assert.That(status, Is.Null);
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_false_if_directions_not_successful()
    {
        var job = _fixture.Create<Job>();
        job.DirectionsSuccessful = false;
        job.WeatherSuccessful = true;
        job.ImagingSuccessful = true;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        Assert.That(status, Is.False);
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_false_if_weather_not_successful()
    {
        var job = _fixture.Create<Job>();
        job.DirectionsSuccessful = true;
        job.WeatherSuccessful = false;
        job.ImagingSuccessful = true;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        Assert.That(status, Is.False);
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_false_if_imaging_not_successful()
    {
        var job = _fixture.Create<Job>();
        job.DirectionsSuccessful = true;
        job.WeatherSuccessful = true;
        job.ImagingSuccessful = false;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        Assert.That(status, Is.False);
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_true_if_all_successful()
    {
        var job = _fixture.Create<Job>();
        job.DirectionsSuccessful = true;
        job.WeatherSuccessful = true;
        job.ImagingSuccessful = true;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        Assert.That(status, Is.True);
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_geocoding()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var successful = !job.GeocodingSuccessful.GetValueOrDefault();
        var geocoding = _fixture.Create<GeocodingResult>();
        await _context.Sut.UpdateJobStatusAsync(job.JobId, successful, geocoding);
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(retrieved?.GeocodingResult, Is.EqualTo(geocoding));
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_geocoding_is_successful()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var successful = !job.GeocodingSuccessful.GetValueOrDefault();
        var geocoding = _fixture.Create<GeocodingResult>();
        await _context.Sut.UpdateJobStatusAsync(job.JobId, successful, geocoding);
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(retrieved?.GeocodingSuccessful, Is.EqualTo(successful));
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_directions()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var successful = !job.DirectionsSuccessful.GetValueOrDefault();
        var directions = _fixture.Create<Directions>();
        await _context.Sut.UpdateJobStatusAsync(job.JobId, successful, directions);
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(JsonSerializer.Serialize(retrieved?.Directions), Is.EqualTo(JsonSerializer.Serialize(directions)));
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_directions_is_successful()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var successful = !job.DirectionsSuccessful.GetValueOrDefault();
        var directions = _fixture.Create<Directions>();
        await _context.Sut.UpdateJobStatusAsync(job.JobId, successful, directions);
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(retrieved?.DirectionsSuccessful, Is.EqualTo(successful));
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_weather()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var successful = !job.WeatherSuccessful.GetValueOrDefault();
        var weather = _fixture.Create<WeatherForecast>();
        await _context.Sut.UpdateJobStatusAsync(job.JobId, successful, weather);
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(JsonSerializer.Serialize(retrieved?.WeatherForecast), Is.EqualTo(JsonSerializer.Serialize(weather)));
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_weather_is_successful()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var successful = !job.WeatherSuccessful.GetValueOrDefault();
        var weather = _fixture.Create<WeatherForecast>();
        await _context.Sut.UpdateJobStatusAsync(job.JobId, successful, weather);
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(retrieved?.WeatherSuccessful, Is.EqualTo(successful));
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_imaging()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var successful = !job.ImagingSuccessful.GetValueOrDefault();
        var imaging = _fixture.Create<ImagingResult>();
        await _context.Sut.UpdateJobStatusAsync(job.JobId, successful, imaging);
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(retrieved?.ImagingResult, Is.EqualTo(imaging));
    }

    [Test]
    public async Task JobRepository_UpdateJobStatusAsync_updates_imaging_is_successful()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var successful = !job.ImagingSuccessful.GetValueOrDefault();
        var imaging = _fixture.Create<ImagingResult>();
        await _context.Sut.UpdateJobStatusAsync(job.JobId, successful, imaging);
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(retrieved?.ImagingSuccessful, Is.EqualTo(successful));
    }

    [Test]
    public async Task JobRepository_DeleteJobByIdAsync_deletes_job()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        await _context.Sut.DeleteJobByIdAsync(job.JobId);
        job = await _context.Sut.GetJobByIdAsync(job.JobId);
        Assert.That(job, Is.Null);
    }

    [Test]
    public async Task JobRepository_DeleteJobByIdAsync_returns_1_if_job_deleted()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var count = await _context.Sut.DeleteJobByIdAsync(job.JobId);
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public async Task JobRepository_DeleteJobByIdAsync_returns_0_if_job_not_deleted()
    {
        var job = _fixture.Create<Job>();
        var count = await _context.Sut.DeleteJobByIdAsync(job.JobId);
        Assert.That(count, Is.EqualTo(0));
    }
}
