using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Db.Cosmos;
using ServiceBusDriver.Db.Entities;

namespace ServiceBusDriver.Db.Repository
{
    public interface IUserRepository : IBaseRepository<UserEntity>
    {
        Task<List<UserEntity>> GetUserWhereAuthUserIdIs(string authUserId, CancellationToken cancellationToken);
    }

    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly ILogger<BaseRepository> _logger;

        public UserRepository(ILogger<BaseRepository> logger, ICosmosDbClient cosmosDbClient, ICosmosSettings settings) : base(logger)
        {
            _logger = logger;
            Container = cosmosDbClient.GetClient().GetContainer(settings.Database, "Users");
        }

        public async Task<List<UserEntity>> GetUserWhereAuthUserIdIs(string authUserId, CancellationToken cancellationToken)
        {
            var searchQuery = SearchByKeyQuery(authUserId);
            var options = new QueryRequestOptions
            {
                MaxItemCount = 1
            };

            var iterator = Container.GetItemQueryIterator<UserEntity>(
                searchQuery,
                null,
                options);

            var results = new List<UserEntity>();

            while (iterator.HasMoreResults)
            {
                var currentResultSet = await iterator.ReadNextAsync(cancellationToken);
                results.AddRange(currentResultSet);
            }

            return results;
        }

        public static QueryDefinition SearchByKeyQuery(string value)
        {
            return new QueryDefinition(@$"SELECT * 
                    FROM Users c
                    WHERE c.AuthUserId = @value")
                .WithParameter("@value", value);
        }

        public Task<List<T>> GetAll<T>(CancellationToken cancellationToken) where T : BaseEntity
        {
            throw new System.NotImplementedException();
        }
    }
}