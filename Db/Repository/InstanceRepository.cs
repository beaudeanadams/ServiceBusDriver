using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Db.Cosmos;
using ServiceBusDriver.Db.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusDriver.Db.Repository
{
    public interface IInstanceRepository : IBaseRepository<InstanceEntity>
    {
        Task<List<InstanceEntity>> GetInstancesWhereAuthUserIdIs(string userId, CancellationToken cancellationToken);
        Task<List<InstanceEntity>> GetInstanceWhereNamespaceIs(string userId, string name, CancellationToken cancellationToken);
    }

    public class InstanceRepository : BaseRepository, IInstanceRepository
    {
        private readonly ILogger<BaseRepository> _logger;

        public InstanceRepository(ICosmosDbClient cosmosDbClient, ILogger<BaseRepository> logger, ICosmosSettings settings) : base(logger)
        {
            Container = cosmosDbClient.GetClient().GetContainer(settings.Database, "Instances");
            _logger = logger;
        }

        public async Task<List<InstanceEntity>> GetInstancesWhereAuthUserIdIs(string userId, CancellationToken cancellationToken)
        {
            var searchQuery = new QueryDefinition(@"SELECT * 
                    FROM Instances c
                    WHERE c.UserId = @value")
                .WithParameter("@value", userId);

            var options = new QueryRequestOptions
            {
            };

            var iterator = Container.GetItemQueryIterator<InstanceEntity>(
                searchQuery,
                null,
                options);

            var results = new List<InstanceEntity>();

            while (iterator.HasMoreResults)
            {
                var currentResultSet = await iterator.ReadNextAsync(cancellationToken);
                results.AddRange(currentResultSet);
            }

            return results;
        }

        public async Task<List<InstanceEntity>> GetInstanceWhereNamespaceIs(string userId, string name, CancellationToken cancellationToken)
        {
            // var query = _iFirestoreDbClient.GetClient().Collection(CollectionName).WhereEqualTo("UserId", userId).WhereEqualTo("Namespace", name);
            // return await QueryRecords<InstanceEntity>(query, cancellationToken);

            var searchQuery = new QueryDefinition(@"SELECT * 
                    FROM Instances c
                    WHERE c.UserId = @value AND  c.Namespace = @name")
                              .WithParameter("@value", userId)
                              .WithParameter("@name", name);

            var options = new QueryRequestOptions
            {
            };

            var iterator = Container.GetItemQueryIterator<InstanceEntity>(
                searchQuery,
                null,
                options);

            var results = new List<InstanceEntity>();

            while (iterator.HasMoreResults)
            {
                var currentResultSet = await iterator.ReadNextAsync(cancellationToken);
                results.AddRange(currentResultSet);
            }

            return results;
        }

        public async Task<List<InstanceEntity>> GetAll<InstanceEntity>(CancellationToken cancellationToken) where InstanceEntity : BaseEntity
        {
            var searchQuery = new QueryDefinition("SELECT * FROM Instances c");

            var options = new QueryRequestOptions
            {
            };

            var iterator = Container.GetItemQueryIterator<InstanceEntity>(
                searchQuery,
                null,
                options);


            var results = new List<InstanceEntity>();

            while (iterator.HasMoreResults)
            {
                var currentResultSet = await iterator.ReadNextAsync(cancellationToken);
                results.AddRange(currentResultSet);
            }

            return results;
        }
    }
}