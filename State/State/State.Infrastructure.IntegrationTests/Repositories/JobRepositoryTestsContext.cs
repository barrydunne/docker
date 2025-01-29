using Microservices.Shared.Mocks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using State.Application.Models;
using State.Infrastructure.Repositories;
using Testcontainers.MongoDb;

namespace State.Infrastructure.IntegrationTests.Repositories;

internal class JobRepositoryTestsContext : IDisposable
{
    private readonly MongoDbContainer _container;
    private readonly string _connectionString;
    private readonly MockCloudSecrets _mockCloudSecrets;
    private readonly MockLogger<JobRepository> _mockLogger;
    private readonly Fixture _fixture;
    private readonly string _databaseName;
    private readonly string _collectionName;

    private bool _disposedValue;

    internal JobRepository Sut { get; }

    public JobRepositoryTestsContext()
    {
        /* These tests require a user and password to be created in the target MongoDB. For example:
         *
         * use admin
         * db.createUser({user: "integration.tests",pwd:"password", roles:["userAdminAnyDatabase", "dbAdminAnyDatabase", "readWriteAnyDatabase"]})
         */

        _container = new MongoDbBuilder()
            .WithImage("mongo:7.0.2")
            .WithName($"JobRepositoryTests.Mongo_{Guid.NewGuid():N}")
            .WithUsername("integration.tests")
            .WithPassword("password")
            .Build();

        _container.StartAsync().GetAwaiter().GetResult();

        _connectionString = _container.GetConnectionString();
        _mockCloudSecrets = new();
        _mockCloudSecrets.WithSecretValue("state", "mongo.connectionstring", _connectionString);

        _mockLogger = new();

        _fixture = new();
        _fixture.Customizations.Add(new JobRepositorySpecimenBuilder());
        _databaseName = _fixture.Create<string>();
        _collectionName = _fixture.Create<string>();

        Sut = new(_mockCloudSecrets, _mockLogger)
        {
            DatabaseName = _databaseName,
            CollectionName = _collectionName
        };
    }

    private IMongoCollection<Job> GetCollection()
    {
        var client = new MongoClient(_connectionString);
        var database = client.GetDatabase(_databaseName);
        return database.GetCollection<Job>(_collectionName);
    }

    private IQueryable<Job> GetCollectionQueryable() => GetCollection().AsQueryable();

    internal JobRepositoryTestsContext DeleteDatabase()
    {
        var client = new MongoClient(_connectionString);
        client.DropDatabase(_databaseName);
        return this;
    }

    internal JobRepositoryTestsContext WithExistingIndex(string indexPropertyName, bool uniqueIndex)
    {
        var collection = GetCollection();
        collection.InsertOne(_fixture.Create<Job>());
        var indexKeys = Builders<Job>.IndexKeys.Ascending(indexPropertyName);
        var indexOptions = new CreateIndexOptions
        {
            Unique = uniqueIndex
        };
        collection.Indexes.CreateOne(new CreateIndexModel<Job>(indexKeys, indexOptions), null);
        return this;
    }

    internal JobRepositoryTestsContext AssertJobSaved(Job job)
    {
        var saved = GetCollectionQueryable().Where(_ => _.JobId == job.JobId).FirstOrDefault();
        saved.ShouldNotBeNull();
        return this;
    }

    internal JobRepositoryTestsContext AssertIndexCreated(string propertyName)
    {
        var indexes = GetCollection().Indexes.List().ToList();
        var indexKeyNames = indexes.Select(_ => string.Join(",", _["key"].AsBsonDocument.Elements.Select(_ => _.Name))).ToArray();
        indexKeyNames.ShouldContain(propertyName);
        return this;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _container.DisposeAsync().GetAwaiter().GetResult();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
