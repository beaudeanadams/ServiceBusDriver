using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Instance;
using ServiceBusDriver.Core.Components.SbClients;
using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Core.Models.Errors;
using ServiceBusDriver.Core.Models.Features.Message;

namespace ServiceBusDriver.Core.Components.Message
{
    public class MessageService : IMessageService
    {
        private readonly ISbAdminService _sbAdminService;
        private readonly ISbClientService _sbClientService;
        private readonly IInstanceService _instanceService;
        private readonly ILogger<MessageService> _logger;

        public MessageService(ISbAdminService sbAdminService, IInstanceService instanceService, ILogger<MessageService> logger, ISbClientService sbClientService)
        {
            _sbAdminService = sbAdminService;
            _instanceService = instanceService;
            _logger = logger;
            _sbClientService = sbClientService;
        }

        public async Task<List<MessageResponse>> FetchMessages(FetchMessagesCommand command, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(FetchMessages));
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var instance = await _instanceService.GetInstanceFull(command.InstanceId, cancellationToken);

            if (instance == null)
            {
                throw SbDriverExceptionFactory.CreateBadRequestException("Invalid Id");
            }

            var adminClient = _sbAdminService.Client(instance.ConnectionString);

            var messagesInQueue = await GetMessagesInQueueCount(command, adminClient, cancellationToken);

            if (messagesInQueue == 0)
            {
                throw new SbDriverException
                {
                    ErrorMessage = new ErrorMessageModel
                    {
                        Code = ErrorConstants.NoMessagesInQueueErrorCode,
                        UserMessageText = string.Format(ErrorConstants.NoMessagesInQueueErrorMessage, command.SubscriptionName)
                    }
                };
            }

            var sbClient = _sbClientService.Client(instance.ConnectionString);

            try
            {
                var subQueue = command.DeadLetterQueue ? SubQueue.DeadLetter : SubQueue.None;

                var receiver = GetServiceBusReceiver(sbClient, command, messagesInQueue, subQueue);

                var maxMessages = command.FetchAll ? messagesInQueue : command.MaxMessages;
                var messages = await GetServiceBusReceivedMessages(receiver, maxMessages, cancellationToken);

                var response = new List<MessageResponse>();
                var totalCount = messages.Count < maxMessages ? messages.Count : maxMessages;

                for (var i = 0; i < totalCount; i++)
                {
                    var message = messages[i];
                    response.Add(new MessageResponse
                    {
                        Message = message,
                        Payload = message.Body.ToString()
                    });
                }

                watch.Stop();
                _logger.LogInformation("Performance - Fetched {0} Messages in {1} ms", totalCount, watch.ElapsedMilliseconds);

                return command.OrderByDescending
                    ? response.OrderByDescending(x => x.Message.EnqueuedSequenceNumber).ToList()
                    : response.OrderBy(x => x.Message.EnqueuedSequenceNumber).ToList();
            }
            catch (Exception e)
            {
                throw new SbDriverException(e.Message)
                {
                    ErrorMessage = new ErrorMessageModel
                    {
                        Code = ErrorConstants.CommunicationsErrorCode,
                        UserMessageText = e.Message,
                        SupportReferenceId = new Guid().ToString()
                    }
                };
            }
        }

        private async Task<List<ServiceBusReceivedMessage>> GetServiceBusReceivedMessages(ServiceBusReceiver receiver, int maxMessages, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(GetServiceBusReceivedMessages));
            var messages = new List<ServiceBusReceivedMessage>();
            var previousSequenceNumber = -1L;
            var count = maxMessages;
            do
            {
                var messageBatch = await receiver.ReceiveMessagesAsync(count, null, cancellationToken);

                if (messageBatch.Count > 0)
                {
                    var sequenceNumber = messageBatch[^1].SequenceNumber;

                    if (sequenceNumber == previousSequenceNumber)
                        break;

                    messages.AddRange(messageBatch);

                    previousSequenceNumber = sequenceNumber;
                }
                else
                {
                    break;
                }

                // Set count to remaining number of messages to fetch
                count = (maxMessages > messages.Count) ? maxMessages - messages.Count : count;
            } while (messages.Count < maxMessages);

            _logger.LogTrace("Finish {0}", nameof(GetServiceBusReceivedMessages));
            return messages;
        }

        private ServiceBusReceiver GetServiceBusReceiver(ServiceBusClient sbClient, FetchMessagesCommand command, int activeMessageCount, SubQueue subQueue)
        {
            _logger.LogTrace("Start {0}", nameof(GetServiceBusReceiver));

            ServiceBusReceiver receiver;

            var prefetchCount = activeMessageCount > MessageConstants.FetchMessagePrefetchCount ? MessageConstants.FetchMessagePrefetchCount : activeMessageCount;
            if (command.ReceiveAndDelete)
            {
                // If Receive and delete, do not prefetch messages.
                prefetchCount = 0;
            }

            if (command.QueueName == null)
            {
                receiver = sbClient.CreateReceiver(command.TopicName, command.SubscriptionName, new ServiceBusReceiverOptions
                {
                    ReceiveMode = command.ReceiveAndDelete ? ServiceBusReceiveMode.ReceiveAndDelete : ServiceBusReceiveMode.PeekLock,
                    PrefetchCount = prefetchCount,
                    SubQueue = subQueue
                });
            }
            else
            {
                receiver = sbClient.CreateReceiver(command.QueueName, new ServiceBusReceiverOptions
                {
                    ReceiveMode = command.ReceiveAndDelete ? ServiceBusReceiveMode.ReceiveAndDelete : ServiceBusReceiveMode.PeekLock,
                    PrefetchCount = prefetchCount,
                    SubQueue = subQueue
                });
            }

            _logger.LogTrace("Finish {0}", nameof(GetServiceBusReceiver));
            return receiver;
        }

        private async Task<int> GetMessagesInQueueCount(FetchMessagesCommand command, ServiceBusAdministrationClient adminClient, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(GetMessagesInQueueCount));

            try
            {
                int messagesCount;
                if (command.QueueName != null)
                {
                    var queueDetails = await adminClient.GetQueueRuntimePropertiesAsync(command.QueueName, cancellationToken);
                    messagesCount = command.DeadLetterQueue
                        ? Convert.ToInt32(queueDetails.Value.DeadLetterMessageCount)
                        : Convert.ToInt32(queueDetails.Value.ActiveMessageCount);
                }
                else
                {
                    var subscriptionRuntimeProperty = await adminClient.GetSubscriptionRuntimePropertiesAsync(command.TopicName,
                                                                                                              command.SubscriptionName, cancellationToken);
                    messagesCount = command.DeadLetterQueue
                        ? Convert.ToInt32(subscriptionRuntimeProperty.Value.DeadLetterMessageCount)
                        : Convert.ToInt32(subscriptionRuntimeProperty.Value.ActiveMessageCount);
                }

                _logger.LogTrace("Finish {0}", nameof(GetMessagesInQueueCount));

                return messagesCount;
            }
            catch (ServiceBusException sbe)
            {
                _logger.LogError(sbe, "Exception while trying to GetActiveMessageCount");
                throw new SbDriverException
                {
                    ErrorMessage = new ErrorMessageModel
                    {
                        Code = ErrorConstants.CommunicationsErrorCode,
                        UserMessageText = $"{sbe.Reason} + {sbe.Message}"
                    }
                };
            }
        }
    }
}