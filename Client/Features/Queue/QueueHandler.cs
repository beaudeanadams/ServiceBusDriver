using Microsoft.AspNetCore.WebUtilities;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Shared.Features.Queue;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServiceBusDriver.Client.Features.Queue
{
    public interface IQueueHandler
    {
        Task<List<QueueResponseDto>> GetQueuesInInstance(string instanceId);
        Task<QueueResponseDto> GetQueue(string instanceId, string queueName);
    }

    public class QueueHandler : IQueueHandler
    {
        private readonly HttpClient _httpClient;

        public QueueHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<QueueResponseDto>> GetQueuesInInstance(string instanceId)
        {
            var url = string.Format(ApiConstants.PathConstants.GetQueuesInInstance, instanceId);

            var response = await _httpClient.GetFromJsonAsync<List<QueueResponseDto>>(url);

            return response;
        }

        public async Task<QueueResponseDto> GetQueue(string instanceId, string queueName)
        {
            var queryParams = new Dictionary<string, string>
            {
                { ApiConstants.QueryConstants.InstanceId, instanceId },
                { ApiConstants.QueryConstants.QueueName, queueName },
            };

            var url = QueryHelpers.AddQueryString(ApiConstants.PathConstants.GetQueue, queryParams);

            var response = await _httpClient.GetFromJsonAsync<QueueResponseDto>(url);

            return response;
        }
    }
}