using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Instance;
using ServiceBusDriver.Core.Components.SbClients;
using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Core.Models.Errors;
using ServiceBusDriver.Core.Models.Features.Topic;
using ServiceBusDriver.Core.Tools;

namespace ServiceBusDriver.Core.Components.Topic
{
    public class TopicService : ITopicService
    {
        private readonly ISbAdminService _sbAdminService;
        private readonly IInstanceService _instanceService;
        private readonly ILogger<TopicService> _logger;

        public TopicService(ISbAdminService sbAdminService, IInstanceService instanceService, ILogger<TopicService> logger)
        {
            _sbAdminService = sbAdminService;
            _instanceService = instanceService;
            _logger = logger;
        }


        public async Task<List<TopicResponse>> ListTopics(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(ListTopics));

            var instances = await _instanceService.ListInstancesFull(cancellationToken);
            var topicFetchTaskList = new List<Task<List<TopicResponse>>>();

            foreach (var instance in instances)
            {
                topicFetchTaskList.Add(GetTopicsForInstance(instance.Id, cancellationToken));
            }

            await Task.WhenAll(topicFetchTaskList);

            var topicList = new List<TopicResponse>();
            foreach (var task in topicFetchTaskList)
            {
                topicList.AddRange(task.Result);
            }

            _logger.LogInformation("Fetched {0} Topics", topicList.Count);
            _logger.LogTrace("Finish {0}", nameof(ListTopics));
            return topicList;
        }

        public async Task<List<TopicResponse>> GetTopicsForInstance(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetTopicsForInstance));
            var instance = await _instanceService.GetInstanceFull(id, cancellationToken);

            if (instance == null)
            {
                throw SbDriverExceptionFactory.CreateBadRequestException("Invalid Id");
            }

            try
            {
                var client = _sbAdminService.Client(instance.ConnectionString);
                var topics = client.GetTopicsAsync(cancellationToken).GetItems();
                var topicRuntimeProperty = client.GetTopicsRuntimePropertiesAsync(cancellationToken).GetItems();

                await Task.WhenAll(topics, topicRuntimeProperty);

                var topicDetailList = new List<TopicResponse>();
                foreach (var topic in topics.Result)
                {
                    var runtimeProperty = topicRuntimeProperty.Result
                        .FirstOrDefault(x => x.Name == topic.Name);
                    topicDetailList.Add(new TopicResponse {RunTimeProperties = runtimeProperty, TopicProperties = topic});
                }

                _logger.LogInformation("Fetched {0} Topics", topicDetailList.Count);
                _logger.LogTrace("Finish {0}", nameof(GetTopicsForInstance));

                return topicDetailList;
            }
            catch (ServiceBusException sbe)
            {
                _logger.LogError(sbe, "Error while fetching data from ServiceBus", nameof(GetTopicsForInstance));

                throw new SbDriverException(sbe.Message)
                {
                    ErrorMessage = new ErrorMessageModel
                    {
                        Code = ErrorConstants.CommunicationsErrorCode,
                        UserMessageText = sbe.Message,
                        SupportReferenceId = new Guid().ToString()
                    }
                };
            }
        }

        public async Task<TopicResponse> GetTopicByName(string instanceId, string topicName, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetTopicByName));

            var topics = await GetTopicsForInstance(instanceId, cancellationToken);

            _logger.LogTrace("Finish {0}", nameof(GetTopicByName));

            return topics.FirstOrDefault(x => x.TopicProperties.Name == topicName);
        }
    }
}