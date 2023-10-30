using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PublicApi.Repository.Models;

namespace PublicApi.Repository.IntegrationTests
{
    internal class JobRepositoryTestsContext
    {
        private readonly string _connectionString;
        private readonly MockCloudSecrets _mockCloudSecrets;
        private readonly MockLogger<JobRepository> _mockLogger;
        private readonly Fixture _fixture;
        private readonly string _databaseName;
        private readonly string _collectionName;

        internal JobRepository Sut { get; }

        public JobRepositoryTestsContext()
        {
            /* These tests require a user and password to be created in the target MongoDB. For example:
             *
             * use admin
             * db.createUser({user: "integration.tests",pwd:"password", roles:["userAdminAnyDatabase", "dbAdminAnyDatabase", "readWriteAnyDatabase"]})
             */
            _connectionString = "mongodb://integration.tests:password@localhost:11017/";
            _mockCloudSecrets = new();
            _mockCloudSecrets.WithSecretValue("publicapi", "mongo.connectionstring", _connectionString);

            _mockLogger = new();

            _fixture = new();
            _databaseName = _fixture.Create<string>();
            _collectionName = _fixture.Create<string>();

            Sut = new(_mockCloudSecrets.Object, _mockLogger.Object)
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

        private IMongoQueryable<Job> GetCollectionQueryable() => GetCollection().AsQueryable();

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
            Assert.That(saved, Is.Not.Null);
            return this;
        }

        internal JobRepositoryTestsContext AssertJobStatus(Guid jobId, JobStatus status)
        {
            var saved = GetCollectionQueryable().Where(_ => _.JobId == jobId).FirstOrDefault();
            Assert.That(saved?.Status, Is.EqualTo(status));
            return this;
        }

        internal JobRepositoryTestsContext AssertJobAdditionalInformation(Guid jobId, string additionalInformation)
        {
            var saved = GetCollectionQueryable().Where(_ => _.JobId == jobId).FirstOrDefault();
            Assert.That(saved?.AdditionalInformation, Is.EqualTo(additionalInformation));
            return this;
        }

        internal JobRepositoryTestsContext AssertIndexCreated(string propertyName)
        {
            var indexes = GetCollection().Indexes.List().ToList();
            var indexKeyNames = indexes.Select(_ => string.Join(",", _["key"].AsBsonDocument.Elements.Select(_ => _.Name))).ToArray();
            Assert.That(indexKeyNames, Does.Contain(propertyName));
            return this;
        }
    }
}
