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
using ServiceBusDriver.Core.Models.Features.Subscription;
using ServiceBusDriver.Core.Tools;

namespace ServiceBusDriver.Core.Components.Subscription
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISbAdminService _sbAdminService;
        private readonly IInstanceService _instanceService;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(ISbAdminService sbAdminService, IInstanceService instanceService, ILogger<SubscriptionService> logger)
        {
            _sbAdminService = sbAdminService;
            _instanceService = instanceService;
            _logger = logger;
        }

        public async Task<List<SubscriptionResponse>> GetSubscriptionsForTopic(string instanceId, string topicName, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetSubscriptionsForTopic));
            var instance = await _instanceService.GetInstanceFull(instanceId, cancellationToken);

            if (instance == null)
            {
                throw SbDriverExceptionFactory.CreateBadRequestException("Invalid Id");
            }

            try
            {
                var adminClient = _sbAdminService.Client(instance.ConnectionString);

                var subscriptions = adminClient.GetSubscriptionsAsync(topicName, cancellationToken).GetItems();
                var subscriptionRuntimeProperties = adminClient.GetSubscriptionsRuntimePropertiesAsync(topicName, cancellationToken).GetItems();
                
                await Task.WhenAll(subscriptions, subscriptionRuntimeProperties).ConfigureAwait(false);

                var response = new List<SubscriptionResponse>();

                foreach (var subscription in subscriptions.Result)
                {
                    response.Add(new SubscriptionResponse
                    {
                        SubscriptionProperties = subscription,
                        RunTimeProperties = subscriptionRuntimeProperties.Result.FirstOrDefault(x => x.SubscriptionName == subscription.SubscriptionName),

                    });
                }

                _logger.LogTrace("Finish {0}", nameof(GetSubscriptionsForTopic));
                return response;
            }
            catch (ServiceBusException sbe)
            {
                _logger.LogError(sbe, "Error while fetching data from ServiceBus", nameof(GetSubscriptionByName));

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

        public async Task<SubscriptionResponse> GetSubscriptionByName(string instanceId, string topicName, string subscriptionName, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetSubscriptionsForTopic));
            var instance = await _instanceService.GetInstanceFull(instanceId, cancellationToken);

            if (instance == null)
            {
                throw SbDriverExceptionFactory.CreateBadRequestException("Invalid Id");
            }

            try
            {
                var adminClient = _sbAdminService.Client(instance.ConnectionString);
                var subscription = adminClient.GetSubscriptionAsync(topicName, subscriptionName, cancellationToken);
                var subscriptionRuntimeProperty = adminClient.GetSubscriptionRuntimePropertiesAsync(topicName, subscriptionName, cancellationToken);
                var subscriptionRules = adminClient.GetRulesAsync(topicName, subscriptionName, cancellationToken).GetItems();

                await Task.WhenAll(subscription, subscriptionRuntimeProperty, subscriptionRules);
                var response = new SubscriptionResponse
                {
                    SubscriptionProperties = subscription.Result,
                    RunTimeProperties = subscriptionRuntimeProperty.Result,
                    Rules = subscriptionRules.Result
                };
                _logger.LogTrace("Finish {0}", nameof(GetSubscriptionByName));
                return response;
            }
            catch (ServiceBusException sbe)
            {
                _logger.LogError(sbe, "Error while fetching data from ServiceBus", nameof(GetSubscriptionByName));

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
    }
}