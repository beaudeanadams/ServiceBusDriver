using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ServiceBusDriver.Db.Entities;
using ServiceBusDriver.Db.Repository;
using ServiceBusDriver.Server.Services.AuthContext;
using ServiceBusDriver.Server.Services.Exceptions;

namespace ServiceBusDriver.Server.Services.Validations
{
    public interface IDbFetchHelper
    {
        Task<InstanceEntity> FetchIfExists(string instanceId, CancellationToken cancellation);
        Task<InstanceEntity> FetchIfBelongsToCurrentUser(string instanceId, CancellationToken cancellation);
        Task<bool> BelongToCurrentUser(string instanceId, CancellationToken cancellation);
    }

    public class DbFetchHelper : IDbFetchHelper
    {
        private readonly IInstanceRepository _instanceRepository;
        private readonly ICurrentUser _currentUser;

        public DbFetchHelper(IInstanceRepository instanceRepository, ICurrentUser currentUser)
        {
            _instanceRepository = instanceRepository;
            _currentUser = currentUser;
        }

        public async Task<InstanceEntity> FetchIfExists(string instanceId, CancellationToken cancellation)
        {
            var result = !string.IsNullOrEmpty(instanceId);
            InstanceEntity instance = null;
            if (result)
            {
                instance = await _instanceRepository.Get<InstanceEntity>(instanceId, cancellation);
                result = instance != null;
            }

            if (!result)
            {
                throw new ValidationException("Instance not found");
            }

            return instance;
        }

        public async Task<InstanceEntity> FetchIfBelongsToCurrentUser(string instanceId, CancellationToken cancellation)
        {
            var instance = await FetchIfExists(instanceId, cancellation);

            if (_currentUser.User.Id != instance.UserId)
            {
                throw AppExceptionFactory.CreateForbiddenException();
            }

            return instance;
        }

        public async Task<bool> BelongToCurrentUser(string instanceId, CancellationToken cancellation)
        {
            var instance = await FetchIfExists(instanceId, cancellation);

            if (_currentUser.User.Id != instance.UserId)
            {
                throw AppExceptionFactory.CreateForbiddenException();
            }

            return true;
        }
    }
}