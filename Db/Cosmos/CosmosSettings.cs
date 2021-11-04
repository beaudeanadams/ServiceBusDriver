using Microsoft.Extensions.Configuration;

namespace ServiceBusDriver.Db.Cosmos
{
    public class CosmosSettings : ICosmosSettings
    {
        public CosmosSettings(IConfiguration configuration)
        {
            var cosmos = configuration.GetSection("Cosmos");
            Account = cosmos.GetSection("Account").Value;
            Key = cosmos.GetSection("Key").Value;
            Database = cosmos.GetSection("Database").Value;
        }

        public string Account { get; set; }
        public string Key { get; set; }
        public string Database { get; set; }
    }
}