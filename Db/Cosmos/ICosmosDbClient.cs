using Microsoft.Azure.Cosmos;

namespace ServiceBusDriver.Db.Cosmos
{
    public interface ICosmosDbClient
    {
        CosmosClient GetClient();
    }
}