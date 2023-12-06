using Microservices.Shared.CloudSecrets;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Linq;
using PublicApi.Application.Models;
using PublicApi.Application.Repositories;

namespace PublicApi.Infrastructure.Repositories;

/// <summary>
/// Provides a concrete IJobRepository using MongoDB.
/// </summary>
public class JobRepository : IJobRepository
{
    private readonly ICloudSecrets _secrets;
    private readonly ILogger _logger;
    private readonly AsyncLazy<MongoClientSettings> _lazyMongoClientSettings;

    private static readonly SemaphoreSlim _verifyIndexSemaphore = new(1);
    private static string? _idempotencyIndexesCheckedCollection = null;

    static JobRepository() => ConventionRegistry.Register("IgnoreExtraElementsConvention", new ConventionPack { new IgnoreExtraElementsConvention(true) }, type => true);

    /// <summary>
    /// Initializes a new instance of the <see cref="JobRepository"/> class.
    /// </summary>
    /// <param name="secrets">The secrets that contain the MongoDB connection string.</param>
    /// <param name="logger">The logger to write to.</param>
    public JobRepository(ICloudSecrets secrets, ILogger<JobRepository> logger)
    {
        _secrets = secrets;
        _logger = logger;

        _lazyMongoClientSettings = new(async () =>
        {
            var connectionString = await _secrets.GetSecretValueAsync("publicapi", "mongo.connectionstring");
            _logger.LogDebug("MongoDB ConnectionString: {ConnectionString}", connectionString);
            var mongoClientSettings = MongoClientSettings.FromConnectionString(connectionString);
            mongoClientSettings.ClusterConfigurator = _ => _.Subscribe<CommandStartedEvent>(e => _logger.LogDebug(e.Command.ToString()));
            return mongoClientSettings;
        });
    }

    /// <summary>
    /// Gets or sets the database name to use for storing jobs.
    /// </summary>
    internal string DatabaseName { get; set; } = "publicapi";

    /// <summary>
    /// Gets or sets the collection name to use for storing jobs.
    /// </summary>
    internal string CollectionName { get; set; } = "jobs";

    /// <inheritdoc/>
    public async Task InsertAsync(Job job, CancellationToken cancellationToken = default)
    {
        var collection = await GetCollectionAsync();
        await collection.InsertOneAsync(job, new InsertOneOptions(), cancellationToken);
        await VerifyIndexesExistAsync(collection, DatabaseName, CollectionName, _logger, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Job?> GetJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Get job by id. [{CorrelationId}]", jobId);
        var collection = await GetCollectionAsync();
        var queryable = collection.AsQueryable();
        return await queryable.Where(_ => _.JobId == jobId).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Guid?> GetJobIdByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Get job by idempotency key {IdempotencyKey}", idempotencyKey);
        var collection = await GetCollectionAsync();

        // The business rule is to allow reuse of idempotency key after 7 days
        var idempotencyCutOff = DateTime.UtcNow.AddDays(-7);
        var queryable = collection.AsQueryable();
        var existingJob = await queryable.Where(_ => _.IdempotencyKey == idempotencyKey && _.CreatedUtc > idempotencyCutOff).FirstOrDefaultAsync(cancellationToken);
        return existingJob?.JobId;
    }

    /// <inheritdoc/>
    public async Task UpdateJobStatusAsync(Guid jobId, JobStatus status, string? additionalInformation, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating job status to {Status}, additional information: {AdditionalInformation}. [{CorrelationId}]", status, additionalInformation, jobId);

        var collection = await GetCollectionAsync();

        var filter = Builders<Job>.Filter.Eq(_ => _.JobId, jobId);
        var update = Builders<Job>.Update
                                  .Set(_ => _.Status, status)
                                  .Set(_ => _.AdditionalInformation, additionalInformation);
        await collection.UpdateOneAsync(filter, update, null, cancellationToken);
    }

    private async Task<MongoClient> CreateMongoClientAsync()
    {
        var mongoClientSettings = await _lazyMongoClientSettings;
        return new MongoClient(mongoClientSettings);
    }

    private async Task<IMongoCollection<Job>> GetCollectionAsync()
    {
        var client = await CreateMongoClientAsync();
        var database = client.GetDatabase(DatabaseName);
        return database.GetCollection<Job>(CollectionName);
    }

    private static async Task VerifyIndexesExistAsync(IMongoCollection<Job> collection, string databaseName, string collectionName, ILogger logger, CancellationToken cancellationToken)
    {
        var currentCollection = $"{databaseName}.{collectionName}";
        if (NeedToCheckIndexes())
        {
            // Do not allow concurrent execution of this code
            await _verifyIndexSemaphore.WaitAsync();
            try
            {
                // Check again to make sure still need to run
                if (NeedToCheckIndexes())
                {
                    var cursor = await collection.Indexes.ListAsync(cancellationToken);
                    var indexes = await cursor.ToListAsync(cancellationToken);
                    var indexKeyNames = indexes.Select(_ => string.Join(",", _["key"].AsBsonDocument.Elements.Select(_ => _.Name))).ToArray();

                    await VerifyIndexExistsAsync(collection, databaseName, collectionName, indexKeyNames, nameof(Job.JobId), true, logger, cancellationToken);
                    await VerifyIndexExistsAsync(collection, databaseName, collectionName, indexKeyNames, nameof(Job.IdempotencyKey), false, logger, cancellationToken);

                    _idempotencyIndexesCheckedCollection = currentCollection;
                }
            }
            finally
            {
                _verifyIndexSemaphore.Release();
            }
        }
        bool NeedToCheckIndexes() => _idempotencyIndexesCheckedCollection != currentCollection;
    }

    private static async Task VerifyIndexExistsAsync(IMongoCollection<Job> collection, string databaseName, string collectionName, string[] indexKeyNames, string indexPropertyName, bool uniqueIndex, ILogger logger, CancellationToken cancellationToken)
    {
        var indexExists = indexKeyNames.Contains(indexPropertyName);
        if (indexExists)
        {
            logger.LogDebug("Index {DatabaseName}/{CollectionName}[{IndexPropertyName}] exists", databaseName, collectionName, indexPropertyName);
            return;
        }

        logger.LogInformation("Creating index {DatabaseName}/{CollectionName}[{IndexPropertyName}]", databaseName, collectionName, indexPropertyName);
        var indexKeys = Builders<Job>.IndexKeys.Ascending(indexPropertyName);
        var indexOptions = new CreateIndexOptions
        {
            Unique = uniqueIndex
        };
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<Job>(indexKeys, indexOptions), null, cancellationToken);
        logger.LogInformation("Index {DatabaseName}/{CollectionName}[{IndexPropertyName}] created", databaseName, collectionName, indexPropertyName);
    }
}
