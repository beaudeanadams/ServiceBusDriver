using System.Collections.Generic;
using ServiceBusDriver.Shared.Features.Queue;
using ServiceBusDriver.Shared.Features.Subscription;
using ServiceBusDriver.Shared.Features.Topic;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Client.UIComponents.Helpers
{
    public class PropertiesHelper
    {

        public static Dictionary<string, string> SetTopicProperties(TopicResponseDto selectedTopic)
        {
            var properties = new Dictionary<string, string>();
            properties.Add(nameof(selectedTopic.TopicStats.SubscriptionCount).ToSpaceSeperated(), selectedTopic.TopicStats.SubscriptionCount.ToString());
            properties.Add(nameof(selectedTopic.TopicStats.SizeInBytes).ToSpaceSeperated(), selectedTopic.TopicStats.SizeInBytes.ToString());
            properties.Add(nameof(selectedTopic.TopicStats.ScheduledMessageCount).ToSpaceSeperated(), selectedTopic.TopicStats.ScheduledMessageCount.ToString());
            properties.Add(nameof(selectedTopic.TopicCheckBoxProperties.EnableBatchedOperations).ToSpaceSeperated(), selectedTopic.TopicCheckBoxProperties.EnableBatchedOperations.ToString());
            properties.Add(nameof(selectedTopic.TopicCheckBoxProperties.EnablePartitioning).ToSpaceSeperated(), selectedTopic.TopicCheckBoxProperties.EnablePartitioning.ToString());
            properties.Add(nameof(selectedTopic.TopicCheckBoxProperties.RequiresDuplicateDetection).ToSpaceSeperated(), selectedTopic.TopicCheckBoxProperties.RequiresDuplicateDetection.ToString());
            properties.Add(nameof(selectedTopic.TopicCheckBoxProperties.SupportOrdering).ToSpaceSeperated(), selectedTopic.TopicCheckBoxProperties.SupportOrdering.ToString());
            properties.Add(nameof(selectedTopic.TopicDateProperties.AccessedAt).ToSpaceSeperated(), selectedTopic.TopicDateProperties.AccessedAt.ToString());
            properties.Add(nameof(selectedTopic.TopicDateProperties.CreatedAt).ToSpaceSeperated(), selectedTopic.TopicDateProperties.CreatedAt.ToString());
            properties.Add(nameof(selectedTopic.TopicDateProperties.UpdatedAt).ToSpaceSeperated(), selectedTopic.TopicDateProperties.UpdatedAt.ToString());
            properties.Add(nameof(selectedTopic.TopicTimeDetails.AutoDeleteOnIdle).ToSpaceSeperated(), selectedTopic.TopicTimeDetails.AutoDeleteOnIdle.ToString());
            properties.Add(nameof(selectedTopic.TopicTimeDetails.DuplicateDetectionHistoryTimeWindow).ToSpaceSeperated(),
                           selectedTopic.TopicTimeDetails.DuplicateDetectionHistoryTimeWindow.ToString());
            properties.Add(nameof(selectedTopic.TopicTimeDetails.DefaultMessageTimeToLive).ToSpaceSeperated(), selectedTopic.TopicTimeDetails.DefaultMessageTimeToLive.ToString());
            return properties;
        }

        public static Dictionary<string, string> SetQueueProperties(QueueResponseDto selectedQueue)
        {
            var properties = new Dictionary<string, string>();
            properties.Add(nameof(selectedQueue.QueueStats.SizeInBytes).ToSpaceSeperated(), selectedQueue.QueueStats.SizeInBytes.ToString());
            properties.Add(nameof(selectedQueue.QueueStats.ScheduledMessageCount).ToSpaceSeperated(), selectedQueue.QueueStats.ScheduledMessageCount.ToString());
            properties.Add(nameof(selectedQueue.QueueCheckBoxProperties.EnableBatchedOperations).ToSpaceSeperated(), selectedQueue.QueueCheckBoxProperties.EnableBatchedOperations.ToString());
            properties.Add(nameof(selectedQueue.QueueCheckBoxProperties.EnablePartitioning).ToSpaceSeperated(), selectedQueue.QueueCheckBoxProperties.EnablePartitioning.ToString());
            properties.Add(nameof(selectedQueue.QueueCheckBoxProperties.RequiresDuplicateDetection).ToSpaceSeperated(), selectedQueue.QueueCheckBoxProperties.RequiresDuplicateDetection.ToString());
            properties.Add(nameof(selectedQueue.QueueCheckBoxProperties.SupportOrdering).ToSpaceSeperated(), selectedQueue.QueueCheckBoxProperties.SupportOrdering.ToString());
            properties.Add(nameof(selectedQueue.QueueDateProperties.AccessedAt).ToSpaceSeperated(), selectedQueue.QueueDateProperties.AccessedAt.ToString());
            properties.Add(nameof(selectedQueue.QueueDateProperties.CreatedAt).ToSpaceSeperated(), selectedQueue.QueueDateProperties.CreatedAt.ToString());
            properties.Add(nameof(selectedQueue.QueueDateProperties.UpdatedAt).ToSpaceSeperated(), selectedQueue.QueueDateProperties.UpdatedAt.ToString());
            properties.Add(nameof(selectedQueue.QueueTimeDetails.AutoDeleteOnIdle).ToSpaceSeperated(), selectedQueue.QueueTimeDetails.AutoDeleteOnIdle.ToString());
            properties.Add(nameof(selectedQueue.QueueTimeDetails.DuplicateDetectionHistoryTimeWindow).ToSpaceSeperated(),
                           selectedQueue.QueueTimeDetails.DuplicateDetectionHistoryTimeWindow.ToString());
            properties.Add(nameof(selectedQueue.QueueTimeDetails.DefaultMessageTimeToLive).ToSpaceSeperated(), selectedQueue.QueueTimeDetails.DefaultMessageTimeToLive.ToString());
            return properties;
        }

        public static Dictionary<string, string> SetSubscriptionProperties(SubscriptionResponseDto selectedSubscription)
        {
            var properties = new Dictionary<string, string>();
            properties.Add(nameof(selectedSubscription.MaxDeliveryCount).ToSpaceSeperated(), selectedSubscription.MaxDeliveryCount.ToString());
            properties.Add(nameof(selectedSubscription.EntityStatus).ToSpaceSeperated(), selectedSubscription.EntityStatus);
            properties.Add(nameof(selectedSubscription.ForwardTo).ToSpaceSeperated(), selectedSubscription.ForwardTo);
            properties.Add(nameof(selectedSubscription.ForwardDeadLetteredMessagesTo).ToSpaceSeperated(), selectedSubscription.ForwardDeadLetteredMessagesTo);
            properties.Add(nameof(selectedSubscription.UserMetadata).ToSpaceSeperated(), selectedSubscription.UserMetadata);
            properties.Add(nameof(selectedSubscription.SubscriptionStats.TotalMessageCount).ToSpaceSeperated(), selectedSubscription.SubscriptionStats.TotalMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionStats.ActiveMessageCount).ToSpaceSeperated(), selectedSubscription.SubscriptionStats.ActiveMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionStats.DeadLetterMessageCount).ToSpaceSeperated(), selectedSubscription.SubscriptionStats.DeadLetterMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionStats.TransferMessageCount).ToSpaceSeperated(), selectedSubscription.SubscriptionStats.TransferMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionStats.TransferDeadLetterMessageCount).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionStats.TransferDeadLetterMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionTimeDetails.LockDuration).ToSpaceSeperated(), selectedSubscription.SubscriptionTimeDetails.LockDuration);
            properties.Add(nameof(selectedSubscription.SubscriptionTimeDetails.DefaultMessageTimeToLive).ToSpaceSeperated(), selectedSubscription.SubscriptionTimeDetails.DefaultMessageTimeToLive);
            properties.Add(nameof(selectedSubscription.SubscriptionTimeDetails.AutoDeleteOnIdle).ToSpaceSeperated(), selectedSubscription.SubscriptionTimeDetails.AutoDeleteOnIdle);
            properties.Add(nameof(selectedSubscription.SubscriptionDateProperties.AccessedAt).ToSpaceSeperated(), selectedSubscription.SubscriptionDateProperties.AccessedAt.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionDateProperties.CreatedAt).ToSpaceSeperated(), selectedSubscription.SubscriptionDateProperties.CreatedAt.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionDateProperties.UpdatedAt).ToSpaceSeperated(), selectedSubscription.SubscriptionDateProperties.UpdatedAt.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionCheckBoxProperties.RequiresSession).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionCheckBoxProperties.RequiresSession.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionCheckBoxProperties.DeadLetteringOnMessageExpiration).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionCheckBoxProperties.DeadLetteringOnMessageExpiration.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionCheckBoxProperties.EnableDeadLetteringOnFilterEvaluationExceptions).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionCheckBoxProperties.EnableDeadLetteringOnFilterEvaluationExceptions.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionCheckBoxProperties.EnableBatchedOperations).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionCheckBoxProperties.EnableBatchedOperations.ToString());
            return properties;
        }
    }
}
