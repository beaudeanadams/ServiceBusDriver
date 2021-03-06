using Newtonsoft.Json;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Shared.Features.Message;
using ServiceBusDriver.Shared.Tools;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusDriver.Client.Features.Message
{
    public interface IMessageHandler
    {
        Task<List<MessageResponseDto>> GetActiveMessages(
            string instanceId,
            string queueName,
            string topicName,
            string subscriptionName,
            int maxMessages = ApiConstants.MessagesConstants.MaxMessages,
            bool receiveAndDelete = false);

        Task<List<MessageResponseDto>> GetDeadLetterMessages(
            string instanceId,
            string queueName,
            string topicName,
            string subscriptionName,
            int maxMessages = ApiConstants.MessagesConstants.MaxMessages,
            bool receiveAndDelete = false);

        Task<List<MessageResponseDto>> GetLastNMessages(
            string instanceId,
            string queueName,
            string topicName,
            string subscriptionName,
            bool deadLetterQueue = false,
            int limit = ApiConstants.MessagesConstants.DefaultMessageLimit);

        Task<List<MessageResponseDto>> SearchMessages(
           string instanceId,
           string queueName,
           string topicName,
           string subscriptionName,
           string searchKey,
           string value,
           bool deadLetterQueue = false);
    }

    public class MessageHandler : IMessageHandler
    {
        private readonly HttpClient _httpClient;

        public MessageHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<MessageResponseDto>> GetActiveMessages(
            string instanceId,
            string queueName,
            string topicName,
            string subscriptionName,
            int maxMessages = ApiConstants.MessagesConstants.MaxMessages,
            bool receiveAndDelete = false)
        {

            var queryParams = new Dictionary<string, string>
            {
                { ApiConstants.QueryConstants.InstanceId, instanceId },
                { ApiConstants.QueryConstants.QueueName, queueName },
                { ApiConstants.QueryConstants.TopicName, topicName },
                { ApiConstants.QueryConstants.SubscriptionName, subscriptionName },
                { ApiConstants.QueryConstants.MaxMessages, maxMessages.ToString() },
                { ApiConstants.QueryConstants.ReceiveAndDelete, receiveAndDelete.ToString() }
            };

            var url = StringExtensions.AddQueryStringWithoutNullCheck(ApiConstants.PathConstants.GetActiveMessages, queryParams);

            var response = await _httpClient.GetFromJsonAsync<List<MessageResponseDto>>(url);

            return response;
        }

        public async Task<List<MessageResponseDto>> GetDeadLetterMessages(
            string instanceId,
            string queueName,
            string topicName,
            string subscriptionName,
            int maxMessages = ApiConstants.MessagesConstants.MaxMessages,
            bool receiveAndDelete = false)
        {
            var queryParams = new Dictionary<string, string>
            {
                { ApiConstants.QueryConstants.InstanceId, instanceId },
                { ApiConstants.QueryConstants.QueueName, queueName },
                { ApiConstants.QueryConstants.TopicName, topicName },
                { ApiConstants.QueryConstants.SubscriptionName, subscriptionName },
                { ApiConstants.QueryConstants.MaxMessages, maxMessages.ToString() },
                { ApiConstants.QueryConstants.ReceiveAndDelete, receiveAndDelete.ToString() }
            };

            var url = StringExtensions.AddQueryStringWithoutNullCheck(ApiConstants.PathConstants.GetDeadLetterMessages, queryParams);

            var response = await _httpClient.GetFromJsonAsync<List<MessageResponseDto>>(url);

            return response;
        }

        public async Task<List<MessageResponseDto>> GetLastNMessages(
            string instanceId,
            string queueName,
            string topicName,
            string subscriptionName,
            bool deadLetterQueue = false,
            int limit = ApiConstants.MessagesConstants.DefaultMessageLimit)
        {
            var queryParams = new Dictionary<string, string>
            {
                { ApiConstants.QueryConstants.InstanceId, instanceId },
                { ApiConstants.QueryConstants.QueueName, queueName },
                { ApiConstants.QueryConstants.TopicName, topicName },
                { ApiConstants.QueryConstants.SubscriptionName, subscriptionName },
                { ApiConstants.QueryConstants.DeadLetterQueue, deadLetterQueue.ToString() }
            };

            var path = string.Format(ApiConstants.PathConstants.GetLastNMessages, limit);
            var url = StringExtensions.AddQueryStringWithoutNullCheck(path, queryParams);
            var response = await _httpClient.GetFromJsonAsync<List<MessageResponseDto>>(url);

            return response;
        }

        public async Task<List<MessageResponseDto>> SearchMessages(
          string instanceId,
          string queueName,
          string topicName,
          string subscriptionName,
          string searchKey,
          string value,
          bool deadLetterQueue = false)
        {
            var searchRequest = new SearchRequest
            {
                InstanceId = instanceId,
                TopicName = topicName,
                QueueName = queueName,
                SubscriptionName = subscriptionName,
                SearchDeadLetter = deadLetterQueue,
                KeyPath = string.IsNullOrWhiteSpace(searchKey) ? null : searchKey,
                Value = value
            };
            var response = await _httpClient.PostAsJsonAsync<SearchRequest>(ApiConstants.PathConstants.SearchMessages, searchRequest, CancellationToken.None);
            var result = await response.Content.ReadAsStringAsync();
            var searchList = JsonConvert.DeserializeObject<List<MessageResponseDto>>(result);

            return searchList;
        }

    }
}