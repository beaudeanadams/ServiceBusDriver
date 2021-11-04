using AutoMapper;
using ServiceBusDriver.Core.Models.Features.Topic;
using ServiceBusDriver.Shared.Features.Topic;

namespace ServiceBusDriver.Server.Mappers
{
    public class TopicProfile : Profile
    {
        public TopicProfile()
        {
            CreateMap<TopicResponse, TopicResponseDto>()
                .ForMember(x => x.Name, cd => cd.MapFrom(map => map.TopicProperties.Name))
                .ForMember(x => x.MaxSizeInMegabytes, cd => cd.MapFrom(map => map.TopicProperties.MaxSizeInMegabytes))
                .ForMember(x => x.UserMetadata, cd => cd.MapFrom(map => map.TopicProperties.UserMetadata))
                // Stats
                .ForPath(x => x.TopicStats.SizeInBytes, cd => cd.MapFrom(map => map.RunTimeProperties.SizeInBytes))
                .ForPath(x => x.TopicStats.SubscriptionCount, cd => cd.MapFrom(map => map.RunTimeProperties.SubscriptionCount))
                .ForPath(x => x.TopicStats.ScheduledMessageCount, cd => cd.MapFrom(map => map.RunTimeProperties.ScheduledMessageCount))
                //Date Properties
                .ForPath(x => x.TopicDateProperties.AccessedAt, cd => cd.MapFrom(map => map.RunTimeProperties.AccessedAt))
                .ForPath(x => x.TopicDateProperties.CreatedAt, cd => cd.MapFrom(map => map.RunTimeProperties.CreatedAt))
                .ForPath(x => x.TopicDateProperties.UpdatedAt, cd => cd.MapFrom(map => map.RunTimeProperties.UpdatedAt))
                //Timespan Properties
                .ForPath(x => x.TopicTimeDetails.DefaultMessageTimeToLive, cd => cd.MapFrom(map => map.TopicProperties.DefaultMessageTimeToLive))
                .ForPath(x => x.TopicTimeDetails.AutoDeleteOnIdle, cd => cd.MapFrom(map => map.TopicProperties.AutoDeleteOnIdle))
                .ForPath(x => x.TopicTimeDetails.DuplicateDetectionHistoryTimeWindow, cd => cd.MapFrom(map => map.TopicProperties.DuplicateDetectionHistoryTimeWindow))
                // Boolean Properties
                .ForPath(x => x.TopicCheckBoxProperties.EnablePartitioning, cd => cd.MapFrom(map => map.TopicProperties.EnablePartitioning))
                .ForPath(x => x.TopicCheckBoxProperties.SupportOrdering, cd => cd.MapFrom(map => map.TopicProperties.SupportOrdering))
                .ForPath(x => x.TopicCheckBoxProperties.EnableBatchedOperations, cd => cd.MapFrom(map => map.TopicProperties.EnableBatchedOperations))
                .ForPath(x => x.TopicCheckBoxProperties.RequiresDuplicateDetection, cd => cd.MapFrom(map => map.TopicProperties.RequiresDuplicateDetection));


        }
    }
}
