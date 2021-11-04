using AutoMapper;
using Azure.Messaging.ServiceBus.Administration;
using ServiceBusDriver.Core.Models.Features.Subscription;
using ServiceBusDriver.Shared.Features.Subscription;

namespace ServiceBusDriver.Server.Mappers
{
    public class SubscriptionMapper : Profile
    {
        public SubscriptionMapper()
        {
            CreateMap<SubscriptionResponse, SubscriptionResponseDto>()
                .ForMember(x => x.SubscriptionName, cd => cd.MapFrom(map => map.SubscriptionProperties.SubscriptionName))
                .ForMember(x => x.TopicName, cd => cd.MapFrom(map => map.SubscriptionProperties.TopicName))
                .ForMember(x => x.MaxDeliveryCount, cd => cd.MapFrom(map => map.SubscriptionProperties.MaxDeliveryCount))
                .ForMember(x => x.ForwardTo, cd => cd.MapFrom(map => map.SubscriptionProperties.ForwardTo))
                .ForMember(x => x.ForwardDeadLetteredMessagesTo, cd => cd.MapFrom(map => map.SubscriptionProperties.ForwardDeadLetteredMessagesTo))
                .ForMember(x => x.EntityStatus, cd => cd.MapFrom(map => map.SubscriptionProperties.Status))
                .ForMember(x => x.UserMetadata, cd => cd.MapFrom(map => map.SubscriptionProperties.UserMetadata))
                // Stats
                .ForPath(x => x.SubscriptionStats.TotalMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.TotalMessageCount))
                .ForPath(x => x.SubscriptionStats.ActiveMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.ActiveMessageCount))
                .ForPath(x => x.SubscriptionStats.DeadLetterMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.DeadLetterMessageCount))
                .ForPath(x => x.SubscriptionStats.TransferMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.TransferMessageCount))
                .ForPath(x => x.SubscriptionStats.TransferDeadLetterMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.TransferDeadLetterMessageCount))
                // Date Properties
                .ForPath(x => x.SubscriptionDateProperties.AccessedAt, cd => cd.MapFrom(map => map.RunTimeProperties.AccessedAt))
                .ForPath(x => x.SubscriptionDateProperties.CreatedAt, cd => cd.MapFrom(map => map.RunTimeProperties.CreatedAt))
                .ForPath(x => x.SubscriptionDateProperties.UpdatedAt, cd => cd.MapFrom(map => map.RunTimeProperties.UpdatedAt))
                // Time Details
                .ForPath(x => x.SubscriptionTimeDetails.LockDuration, cd => cd.MapFrom(map => map.SubscriptionProperties.LockDuration))
                .ForPath(x => x.SubscriptionTimeDetails.DefaultMessageTimeToLive, cd => cd.MapFrom(map => map.SubscriptionProperties.DefaultMessageTimeToLive))
                .ForPath(x => x.SubscriptionTimeDetails.AutoDeleteOnIdle, cd => cd.MapFrom(map => map.SubscriptionProperties.AutoDeleteOnIdle))
                // Boolean Properties
                .ForPath(x => x.SubscriptionCheckBoxProperties.RequiresSession, cd => cd.MapFrom(map => map.SubscriptionProperties.RequiresSession))
                .ForPath(x => x.SubscriptionCheckBoxProperties.DeadLetteringOnMessageExpiration, cd => cd.MapFrom(map => map.SubscriptionProperties.DeadLetteringOnMessageExpiration))
                .ForPath(x => x.SubscriptionCheckBoxProperties.EnableBatchedOperations, cd => cd.MapFrom(map => map.SubscriptionProperties.EnableBatchedOperations))
                .ForPath(x => x.SubscriptionCheckBoxProperties.EnableDeadLetteringOnFilterEvaluationExceptions,
                    cd => cd.MapFrom(map => map.SubscriptionProperties.EnableDeadLetteringOnFilterEvaluationExceptions));


            CreateMap<RuleProperties, SubscriptionRule>()
                .ForMember(x => x.Name, cd => cd.MapFrom(map => map.Name))
                .ForMember(x => x.Action, cd => cd.MapFrom(map => map.Action.ToString()))
                .ForMember(x => x.RuleFilter, cd => cd.MapFrom(map => map.Filter.ToString()));
        }
    }
}