using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Shared.Features.Subscription;

namespace ServiceBusDriver.Client.Features.Subscription
{
    public interface ISubscriptionHandler
    {
        Task<SubscriptionResponseDto> GetSubscription(string instanceId, string topicName, string subscriptionName);
        Task<List<SubscriptionResponseDto>> GetSubscriptionsInTopic(string instanceId, string topicName);
    }

    public class SubscriptionHandler : ISubscriptionHandler
    {
        private readonly HttpClient _httpClient;

        public SubscriptionHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SubscriptionResponseDto> GetSubscription(string instanceId, string topicName, string subscriptionName)
        {
            var queryParams = new Dictionary<string, string>
            {
                { ApiConstants.QueryConstants.InstanceId, instanceId },
                { ApiConstants.QueryConstants.TopicName, topicName },
                { ApiConstants.QueryConstants.SubscriptionName, subscriptionName }
            };

            var url = QueryHelpers.AddQueryString(ApiConstants.PathConstants.GetSubscription, queryParams);

            var response = await _httpClient.GetFromJsonAsync<SubscriptionResponseDto>(url);

            return response;
        }

        public async Task<List<SubscriptionResponseDto>> GetSubscriptionsInTopic(string instanceId, string topicName)
        {
            var queryParams = new Dictionary<string, string>
            {
                { ApiConstants.QueryConstants.InstanceId, instanceId },
                { ApiConstants.QueryConstants.TopicName, topicName }
            };

            var url = QueryHelpers.AddQueryString(ApiConstants.PathConstants.GetSubscriptionsInTopic, queryParams);

            var response = await _httpClient.GetFromJsonAsync<List<SubscriptionResponseDto>>(url);

            return response;
        }
    }
}