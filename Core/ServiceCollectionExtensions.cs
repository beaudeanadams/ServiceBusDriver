using Microsoft.Extensions.DependencyInjection;
using ServiceBusDriver.Core.Components.Connection;
using ServiceBusDriver.Core.Components.Instance;
using ServiceBusDriver.Core.Components.Message;
using ServiceBusDriver.Core.Components.SbClients;
using ServiceBusDriver.Core.Components.Search;
using ServiceBusDriver.Core.Components.Subscription;
using ServiceBusDriver.Core.Components.Topic;

namespace ServiceBusDriver.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServiceBusDriverCore(this IServiceCollection services)
        {
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<IInstanceService, InstanceService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<ISearchService, SearchService>();

            services.AddScoped<ISbAdminService, SbAdminService>();
            services.AddScoped<ISbClientService, SbClientService>();
        }
    }
}