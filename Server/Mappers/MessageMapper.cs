using AutoMapper;
using ServiceBusDriver.Core.Models.Features.Message;
using ServiceBusDriver.Shared.Features.Message;

namespace ServiceBusDriver.Server.Mappers
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<MessageResponse, MessageResponseDto>()
                .ForMember(x => x.MessageId, cd => cd.MapFrom(map => map.Message.MessageId))
                .ForMember(x => x.Subject, cd => cd.MapFrom(map => map.Message.Subject))
                .ForMember(x => x.SequenceNumber, cd => cd.MapFrom(map => map.Message.SequenceNumber))
                .ForMember(x => x.DeliveryCount, cd => cd.MapFrom(map => map.Message.DeliveryCount))
                .ForMember(x => x.Payload, cd => cd.MapFrom(map => map.Payload))
                .ForMember(x => x.ContentType, cd => cd.MapFrom(map => map.Message.ContentType))
                .ForMember(x => x.EnqueuedTime, cd => cd.MapFrom(map => map.Message.EnqueuedTime))
                .ForMember(x => x.TimeToLive, cd => cd.MapFrom(map => map.Message.TimeToLive))
                .ForMember(x => x.CorrelationId, cd => cd.MapFrom(map => map.Message.CorrelationId))
                .ForMember(x => x.EnqueuedSequenceNumber, cd => cd.MapFrom(map => map.Message.EnqueuedSequenceNumber))
                .ForMember(x => x.ApplicationProperties, cd => cd.MapFrom(map => map.Message.ApplicationProperties))

                // Stats
                .ForPath(x => x.MessageSecondaryProperties.To, cd => cd.MapFrom(map => map.Message.To))
                .ForPath(x => x.MessageSecondaryProperties.ReplyTo, cd => cd.MapFrom(map => map.Message.ReplyTo))
                .ForPath(x => x.MessageSecondaryProperties.PartitionKey, cd => cd.MapFrom(map => map.Message.PartitionKey))
                .ForPath(x => x.MessageSecondaryProperties.TransactionPartitionKey, cd => cd.MapFrom(map => map.Message.TransactionPartitionKey))
                .ForPath(x => x.MessageSecondaryProperties.SessionId, cd => cd.MapFrom(map => map.Message.SessionId))
                .ForPath(x => x.MessageSecondaryProperties.ReplyToSessionId, cd => cd.MapFrom(map => map.Message.ReplyToSessionId))
                .ForPath(x => x.MessageSecondaryProperties.LockToken, cd => cd.MapFrom(map => map.Message.LockToken))

                //Time Properties
                .ForPath(x => x.MessageTimeProperties.ScheduledEnqueueTime, cd => cd.MapFrom(map => map.Message.ScheduledEnqueueTime))
                .ForPath(x => x.MessageTimeProperties.LockedUntil, cd => cd.MapFrom(map => map.Message.LockedUntil))
                .ForPath(x => x.MessageTimeProperties.ExpiresAt, cd => cd.MapFrom(map => map.Message.ExpiresAt))

                //Dead Letter Properties
                .ForPath(x => x.DeadLetterProperties.DeadLetterSource, cd => cd.MapFrom(map => map.Message.DeadLetterSource))
                .ForPath(x => x.DeadLetterProperties.DeadLetterReason, cd => cd.MapFrom(map => map.Message.DeadLetterReason))
                .ForPath(x => x.DeadLetterProperties.DeadLetterErrorDescription, cd => cd.MapFrom(map => map.Message.DeadLetterErrorDescription));
        }


    }
}
