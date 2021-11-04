using ServiceBusDriver.Db.Entities;

namespace ServiceBusDriver.Server.Services.AuthContext
{
    public interface ICurrentUser
    {
        UserEntity User { get; set; }
    }
}