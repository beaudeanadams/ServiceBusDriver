using AutoMapper;
using ServiceBusDriver.Db.Entities;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Server.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserEntity, UserResponseDto>().ReverseMap();
        }
    }
}