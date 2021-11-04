using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Instance;
using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Core.Models.Features.Instance;
using ServiceBusDriver.Db.Entities;
using ServiceBusDriver.Db.Repository;
using ServiceBusDriver.Server.Services.Password;
using ServiceBusDriver.Shared.Features.Error;

namespace ServiceBusDriver.Server.Services
{
    public class MyInstanceService : InstanceService
    {
        private readonly ILogger<MyInstanceService> _logger;
        private readonly IInstanceRepository _instanceRepository;
        private readonly IAesEncryptService _aesEncryptService;


        public MyInstanceService(ILogger<MyInstanceService> logger, IInstanceRepository instanceRepository, IAesEncryptService aesEncryptService) : base(logger)
        {
            _logger = logger;
            _instanceRepository = instanceRepository;
            _aesEncryptService = aesEncryptService;
        }


        /// <summary>
        /// List instances stored in DB
        /// </summary>
        public override async Task<List<InstanceResponse>> ListInstances(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(ListInstances));

            var sbInstances = await ListInstancesFull(cancellationToken);

            var instances = sbInstances.Select(i => new InstanceResponse { Id = i.Id, Name = i.Name })
                .ToList();

            _logger.LogInformation("Fetched {0} instances", instances.Count);

            _logger.LogTrace("Finish {0}", nameof(ListInstances));

            return await Task.FromResult(instances).ConfigureAwait(false);
        }

        public override async Task<List<ServiceBusInstanceModel>> ListInstancesFull(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(ListInstancesFull));

            var instances = await _instanceRepository.GetAll<InstanceEntity>(cancellationToken);

            _logger.LogInformation("Fetched {0} instances", instances.Count);

            var serviceBusInstanceModelList = instances
                .Select(instance => new ServiceBusInstanceModel
                {
                    Id = instance.Id,
                    Name = instance.Namespace,
                    ConnectionString = _aesEncryptService.Decrypt(instance.RawConnectionString)
                }).ToList();

            _logger.LogTrace("Finish {0}", nameof(ListInstances));

            return serviceBusInstanceModelList;
        }

        /// <summary>
        /// Get Instance from Id
        /// </summary>
        /// <param name="id">Instance Id.</param>
        public override async Task<InstanceResponse> GetInstance(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetInstance));

            var sbInstance = await GetInstanceFull(id, cancellationToken);
            var instance = new InstanceResponse();
            if (sbInstance != null)
            {
                instance = new InstanceResponse { Id = sbInstance.Id, Name = sbInstance.Name };
                _logger.LogInformation("Fetched instance with Id {0}", sbInstance.Id);
            }
            else
            {
                _logger.LogWarning("Instance with {0} not found", id);
            }

            _logger.LogTrace("Finish {0}", nameof(ListInstances));


            return await Task.FromResult(instance).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Instance from Id With ConnectionString
        /// </summary>
        /// <param name="id">Instance Id.</param>
        public override async Task<ServiceBusInstanceModel> GetInstanceFull(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetInstanceFull));

            var instance = await _instanceRepository.Get<InstanceEntity>(id, cancellationToken);

            try
            {
                if (instance == null)
                {
                    _logger.LogWarning("Instance with {0} not found", id);

                    throw new AppException()
                    {
                        ErrorMessage = new AppErrorMessageDto
                        {
                            Code = ErrorConstants.CommunicationsErrorCode,
                            UserMessageText = $"Instance with {id} not found"
                        }
                    };
                }

                _logger.LogTrace("Finish {0}", nameof(GetInstanceFull));

                var result = new ServiceBusInstanceModel
                {
                    Id = instance.Id,
                    Name = instance.Namespace,
                    ConnectionString = _aesEncryptService.Decrypt(instance.RawConnectionString)
                };

                return result;
            }
            catch (Exception e) when (!(e is AppException))
            {
                _logger.LogError(e, "Error while fetching instances", nameof(GetInstanceFull));

                throw new AppException(e.Message)
                {
                    ErrorMessage = new AppErrorMessageDto
                    {
                        Code = ErrorConstants.CommunicationsErrorCode,
                        UserMessageText = e.Message,
                        SupportReferenceId = new Guid().ToString()
                    }
                };
            }
        }
    }
}