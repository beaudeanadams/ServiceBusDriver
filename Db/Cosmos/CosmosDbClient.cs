using Microsoft.Azure.Cosmos;

namespace ServiceBusDriver.Db.Cosmos
{
    public class CosmosDbClient : ICosmosDbClient
    {
        private readonly ICosmosSettings _settings;
        public static CosmosClient CosmosClient;

        public CosmosDbClient(ICosmosSettings settings)
        {
            _settings = settings;
        }

        public CosmosClient GetClient()
        {
            if (CosmosClient != null) return CosmosClient;

            CosmosClient = new CosmosClient(_settings.Account, _settings.Key);

            return CosmosClient;
        }
    }
}