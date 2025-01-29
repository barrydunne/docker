using Microservices.Shared.Events;
using State.Application.Models;
using System.Text.Json;

namespace State.Infrastructure.IntegrationTests.Repositories;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "JobRepository")]
public class JobRepositoryTests : IDisposable
{
    private readonly Fixture _fixture;
    private readonly JobRepositoryTestsContext _context;
    private bool _disposedValue;

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
        JsonSerializer.Serialize(retrieved).ShouldBe(JsonSerializer.Serialize(job));
    }

    [Test]
    public async Task JobRepository_GetJobByIdAsync_returns_null_if_unknown()
    {
        var job = _fixture.Create<Job>();
        var retrieved = await _context.Sut.GetJobByIdAsync(job.JobId);
        retrieved.ShouldBeNull();
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
        status.ShouldBeNull();
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_null_if_directions_not_complete()
    {
        var job = _fixture.Create<Job>();
        job.DirectionsSuccessful = null;
        job.Directions = null;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        status.ShouldBeNull();
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_null_if_weather_not_complete()
    {
        var job = _fixture.Create<Job>();
        job.WeatherSuccessful = null;
        job.WeatherForecast = null;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        status.ShouldBeNull();
    }

    [Test]
    public async Task JobRepository_GetJobCompletionStatusAsync_returns_null_if_imaging_not_complete()
    {
        var job = _fixture.Create<Job>();
        job.ImagingSuccessful = null;
        job.ImagingResult = null;
        await _context.Sut.InsertAsync(job);
        var status = await _context.Sut.GetJobCompletionStatusAsync(job.JobId);
        status.ShouldBeNull();
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
        status.ShouldBe(false);
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
        status.ShouldBe(false);
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
        status.ShouldBe(false);
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
        status.ShouldBe(true);
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
        retrieved?.GeocodingResult.ShouldBe(geocoding);
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
        retrieved?.GeocodingSuccessful.ShouldBe(successful);
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
        JsonSerializer.Serialize(retrieved?.Directions).ShouldBe(JsonSerializer.Serialize(directions));
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
        retrieved?.DirectionsSuccessful.ShouldBe(successful);
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
        JsonSerializer.Serialize(retrieved?.WeatherForecast).ShouldBe(JsonSerializer.Serialize(weather));
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
        retrieved?.WeatherSuccessful.ShouldBe(successful);
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
        retrieved?.ImagingResult.ShouldBe(imaging);
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
        retrieved?.ImagingSuccessful.ShouldBe(successful);
    }

    [Test]
    public async Task JobRepository_DeleteJobByIdAsync_deletes_job()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        await _context.Sut.DeleteJobByIdAsync(job.JobId);
        job = await _context.Sut.GetJobByIdAsync(job.JobId);
        job.ShouldBeNull();
    }

    [Test]
    public async Task JobRepository_DeleteJobByIdAsync_returns_1_if_job_deleted()
    {
        var job = _fixture.Create<Job>();
        await _context.Sut.InsertAsync(job);
        var count = await _context.Sut.DeleteJobByIdAsync(job.JobId);
        count.ShouldBe(1);
    }

    [Test]
    public async Task JobRepository_DeleteJobByIdAsync_returns_0_if_job_not_deleted()
    {
        var job = _fixture.Create<Job>();
        var count = await _context.Sut.DeleteJobByIdAsync(job.JobId);
        count.ShouldBe(0);
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
