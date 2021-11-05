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
using ServiceBusDriver.Core.Models.Features.Queue;
using ServiceBusDriver.Core.Tools;

namespace ServiceBusDriver.Core.Components.Queue
{
    public class QueueService : IQueueService
    {
        private readonly ISbAdminService _sbAdminService;
        private readonly IInstanceService _instanceService;
        private readonly ILogger<QueueService> _logger;

        public QueueService(ISbAdminService sbAdminService, IInstanceService instanceService, ILogger<QueueService> logger)
        {
            _sbAdminService = sbAdminService;
            _instanceService = instanceService;
            _logger = logger;
        }


        public async Task<List<QueueResponse>> ListQueues(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(ListQueues));

            var instances = await _instanceService.ListInstancesFull(cancellationToken);
            var queueFetchTaskList = new List<Task<List<QueueResponse>>>();

            foreach (var instance in instances)
            {
                queueFetchTaskList.Add(GetQueuesForInstance(instance.Id, cancellationToken));
            }

            await Task.WhenAll(queueFetchTaskList);
            
            var queueList = new List<QueueResponse>();
            foreach (var task in queueFetchTaskList)
            {
                queueList.AddRange(task.Result);
            }

            _logger.LogInformation("Fetched {0} Queues", queueList.Count);
            _logger.LogTrace("Finish {0}", nameof(ListQueues));
            return queueList;
        }

        public async Task<List<QueueResponse>> GetQueuesForInstance(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetQueuesForInstance));
            var instance = await _instanceService.GetInstanceFull(id, cancellationToken);

            if (instance == null)
            {
                throw SbDriverExceptionFactory.CreateBadRequestException("Invalid Id");
            }

            try
            {
                var client = _sbAdminService.Client(instance.ConnectionString);
                var queues = client.GetQueuesAsync(cancellationToken).GetItems();
                var queueRuntimeProperty = client.GetQueuesRuntimePropertiesAsync(cancellationToken).GetItems();

                await Task.WhenAll(queues, queueRuntimeProperty);

                var queueDetailList = new List<QueueResponse>();
                foreach (var Queue in queues.Result)
                {
                    var runtimeProperty = queueRuntimeProperty.Result
                        .FirstOrDefault(x => x.Name == Queue.Name);
                    queueDetailList.Add(new QueueResponse {RunTimeProperties = runtimeProperty, QueueProperties = Queue});
                }

                _logger.LogInformation("Fetched {0} Queues", queueDetailList.Count);
                _logger.LogTrace("Finish {0}", nameof(GetQueuesForInstance));

                return queueDetailList;
            }
            catch (ServiceBusException sbe)
            {
                _logger.LogError(sbe, "Error while fetching data from ServiceBus", nameof(GetQueuesForInstance));

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

        public async Task<QueueResponse> GetQueueByName(string instanceId, string queueName, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetQueueByName));

            var queues = await GetQueuesForInstance(instanceId, cancellationToken);

            _logger.LogTrace("Finish {0}", nameof(GetQueueByName));

            return queues.FirstOrDefault(x => x.QueueProperties.Name == queueName);
        }
    }
}