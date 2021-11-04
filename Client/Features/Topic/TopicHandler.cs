using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Shared.Features.Topic;

namespace ServiceBusDriver.Client.Features.Topic
{
    public interface ITopicHandler
    {
        Task<List<TopicResponseDto>> GetTopicsInInstance(string instanceId);
        Task<TopicResponseDto> GetTopic(string instanceId, string topicName);
    }

    public class TopicHandler : ITopicHandler
    {
        private readonly HttpClient _httpClient;

        public TopicHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TopicResponseDto>> GetTopicsInInstance(string instanceId)
        {
            var url = string.Format(ApiConstants.PathConstants.GetTopicsInInstance, instanceId);

            var response = await _httpClient.GetFromJsonAsync<List<TopicResponseDto>>(url);

            return response;
        }

        public async Task<TopicResponseDto> GetTopic(string instanceId, string topicName)
        {
            var queryParams = new Dictionary<string, string>
            {
                { ApiConstants.QueryConstants.InstanceId, instanceId },
                { ApiConstants.QueryConstants.TopicName, topicName },
            };

            var url = QueryHelpers.AddQueryString(ApiConstants.PathConstants.GetTopic, queryParams);

            var response = await _httpClient.GetFromJsonAsync<TopicResponseDto>(url);

            return response;
        }
    }
}