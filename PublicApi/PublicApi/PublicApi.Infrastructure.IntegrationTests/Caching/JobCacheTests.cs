using PublicApi.Application.Models;
using PublicApi.Infrastructure.Caching;

namespace PublicApi.Infrastructure.IntegrationTests.Caching;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Caching")]
internal class JobCacheTests : IDisposable
{
    private readonly Fixture _fixture = new();
    private readonly JobCache _sut = new();
    private bool _disposedValue = false;

    [Test]
    public void JobCache_Get_returns_null_for_unknown_key()
        => Assert.That(_sut.Get(_fixture.Create<Guid>()), Is.Null);

    [Test]
    public void JobCache_Get_returns_job_for_known_key()
    {
        var job = _fixture.Create<Job>();
        _sut.Set(job, TimeSpan.FromSeconds(1));
        var cached = _sut.Get(job.JobId);
        Assert.That(cached, Is.EqualTo(job));
    }

    [Test]
    public void JobCache_Set_stores_job()
    {
        var job = _fixture.Create<Job>();
        _sut.Set(job, TimeSpan.FromSeconds(1));
        var cached = _sut.Get(job.JobId);
        Assert.That(cached, Is.EqualTo(job));
    }

    [Test]
    public void JobCache_Remove_removes_job()
    {
        var job = _fixture.Create<Job>();
        _sut.Set(job, TimeSpan.FromSeconds(1));
        _sut.Remove(job.JobId);
        var cached = _sut.Get(job.JobId);
        Assert.That(cached, Is.Null);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _sut.Dispose();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
