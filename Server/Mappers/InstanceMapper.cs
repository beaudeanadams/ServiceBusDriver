using AutoMapper;
using ServiceBusDriver.Db.Entities;
using ServiceBusDriver.Shared.Features.Instance;

namespace ServiceBusDriver.Server.Mappers
{
    public class InstanceProfile : Profile
    {
        public InstanceProfile()
        {
            CreateMap<InstanceEntity, InstanceResponseDto>()
                .ForMember(x => x.Name, cd => cd.MapFrom(map => map.Namespace));
        }
    }
}