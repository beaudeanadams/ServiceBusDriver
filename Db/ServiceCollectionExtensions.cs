using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusDriver.Db.Cosmos;
using ServiceBusDriver.Db.Repository;

namespace ServiceBusDriver.Db
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFirestoreDb(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<ICosmosSettings, CosmosSettings>(x => new CosmosSettings(configuration));

            services.AddSingleton<ICosmosDbClient, CosmosDbClient>();


            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInstanceRepository, InstanceRepository>();
        }
    }
}