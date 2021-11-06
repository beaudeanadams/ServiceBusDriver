using AutoMapper;
using ServiceBusDriver.Core.Models.Features.Queue;
using ServiceBusDriver.Shared.Features.Queue;

namespace ServiceBusDriver.Server.Mappers
{
    public class QueueProfile : Profile
    {
        public QueueProfile()
        {
            CreateMap<QueueResponse, QueueResponseDto>()
                .ForMember(x => x.Name, cd => cd.MapFrom(map => map.QueueProperties.Name))
                .ForMember(x => x.MaxDeliveryCount, cd => cd.MapFrom(map => map.QueueProperties.MaxDeliveryCount))
                .ForMember(x => x.EntityStatus, cd => cd.MapFrom(map => map.QueueProperties.Status.ToString()))
                .ForMember(x => x.ForwardTo, cd => cd.MapFrom(map => map.QueueProperties.ForwardTo))
                .ForMember(x => x.ForwardDeadLetteredMessagesTo, cd => cd.MapFrom(map => map.QueueProperties.ForwardDeadLetteredMessagesTo))
                .ForMember(x => x.MaxSizeInMegabytes, cd => cd.MapFrom(map => map.QueueProperties.MaxSizeInMegabytes))
                .ForMember(x => x.UserMetadata, cd => cd.MapFrom(map => map.QueueProperties.UserMetadata))
                // Stats
                .ForPath(x => x.QueueStats.SizeInBytes, cd => cd.MapFrom(map => map.RunTimeProperties.SizeInBytes))
                .ForPath(x => x.QueueStats.ScheduledMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.ScheduledMessageCount))
                .ForPath(x => x.QueueStats.TotalMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.TotalMessageCount))
                .ForPath(x => x.QueueStats.ActiveMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.ActiveMessageCount))
                .ForPath(x => x.QueueStats.DeadLetterMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.DeadLetterMessageCount))
                .ForPath(x => x.QueueStats.TransferMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.TransferMessageCount))
                .ForPath(x => x.QueueStats.TransferDeadLetterMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.TransferDeadLetterMessageCount))
                //Date Properties
                .ForPath(x => x.QueueDateProperties.AccessedAt, cd => cd.MapFrom(map => map.RunTimeProperties.AccessedAt))
                .ForPath(x => x.QueueDateProperties.CreatedAt, cd => cd.MapFrom(map => map.RunTimeProperties.CreatedAt))
                .ForPath(x => x.QueueDateProperties.UpdatedAt, cd => cd.MapFrom(map => map.RunTimeProperties.UpdatedAt))
                //Timespan Properties
                .ForPath(x => x.QueueTimeDetails.DefaultMessageTimeToLive, cd => cd.MapFrom(map => map.QueueProperties.DefaultMessageTimeToLive))
                .ForPath(x => x.QueueTimeDetails.AutoDeleteOnIdle, cd => cd.MapFrom(map => map.QueueProperties.AutoDeleteOnIdle))
                .ForPath(x => x.QueueTimeDetails.DuplicateDetectionHistoryTimeWindow, cd => cd.MapFrom(map => map.QueueProperties.DuplicateDetectionHistoryTimeWindow))
                .ForPath(x => x.QueueTimeDetails.LockDuration, cd => cd.MapFrom(map => map.QueueProperties.LockDuration))
                // Boolean Properties
                .ForPath(x => x.QueueCheckBoxProperties.EnablePartitioning, cd => cd.MapFrom(map => map.QueueProperties.EnablePartitioning))
                .ForPath(x => x.QueueCheckBoxProperties.EnableBatchedOperations, cd => cd.MapFrom(map => map.QueueProperties.EnableBatchedOperations))
                .ForPath(x => x.QueueCheckBoxProperties.RequiresSession, cd => cd.MapFrom(map => map.QueueProperties.RequiresSession))
                .ForPath(x => x.QueueCheckBoxProperties.RequiresDuplicateDetection, cd => cd.MapFrom(map => map.QueueProperties.RequiresDuplicateDetection))
                .ForPath(x => x.QueueCheckBoxProperties.DeadLetteringOnMessageExpiration, cd => cd.MapFrom(map => map.QueueProperties.DeadLetteringOnMessageExpiration));
        }
    }
}