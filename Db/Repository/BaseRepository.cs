using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServiceBusDriver.Db.Cosmos;

namespace ServiceBusDriver.Db.Repository
{
    public class BaseRepository
    {
        public Container Container;
        private readonly ILogger<BaseRepository> _logger;

        public BaseRepository(ILogger<BaseRepository> logger)
        {
            _logger = logger;
        }

        public async Task<T> Add<T>(T record, CancellationToken cancellationToken) where T : BaseEntity
        {
            try
            {
                if (string.IsNullOrWhiteSpace(record.Id))
                {
                    record.Id = Guid.NewGuid().ToString();
                }

                await Container.CreateItemAsync<T>(record, new PartitionKey(record.Id), cancellationToken: cancellationToken);
                return record;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<bool> Update<T>(T record, CancellationToken cancellationToken) where T : BaseEntity
        {
            await Container.UpsertItemAsync<T>(record, new PartitionKey(record.Id), cancellationToken: cancellationToken);
            return true;
        }

        public async Task<bool> Delete<T>(T record, CancellationToken cancellationToken) where T : BaseEntity
        {
            await Container.DeleteItemAsync<T>(record.Id, new PartitionKey(record.Id), cancellationToken: cancellationToken);
            return true;
        }

        public async Task<T> Get<T>(string id, CancellationToken cancellationToken) where T : BaseEntity
        {
            try
            {
                var response = await Container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<T>> GetAll<T>(string queryString, CancellationToken cancellationToken) where T : BaseEntity
        {
            var query = Container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync(cancellationToken);

                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}