using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Core.Models.Errors;
using ServiceBusDriver.Core.Models.Features.Instance;

namespace ServiceBusDriver.Core.Components.Instance
{
    public class InstanceService : IInstanceService
    {
        private readonly ILogger<InstanceService> _logger;

        public InstanceService(ILogger<InstanceService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// List instances stored in DB
        /// </summary>
        public virtual async Task<List<InstanceResponse>> ListInstances(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(ListInstances));

            var sbInstances = await ListInstancesFull(cancellationToken);

            var instances = sbInstances.Select(i => new InstanceResponse {Id = i.Id, Name = i.Name})
                .ToList();

            _logger.LogInformation("Fetched {0} instances", instances.Count);

            _logger.LogTrace("Finish {0}", nameof(ListInstances));

            return await Task.FromResult(instances).ConfigureAwait(false);
        }

        /// <summary>
        /// List instances stored in DB with ConnectionString
        /// </summary>
        public virtual async Task<List<ServiceBusInstanceModel>> ListInstancesFull(CancellationToken cancellationToken = default)

        {
            _logger.LogTrace("Start {0}", nameof(ListInstancesFull));

            var instances = InstanceRepository.GetInstances();
            _logger.LogInformation("Fetched {0} instances", instances.Count);

            _logger.LogTrace("Finish {0}", nameof(ListInstances));

            return await Task.FromResult(instances).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Instance from Id
        /// </summary>
        /// <param name="id">Instance Id.</param>
        public virtual async Task<InstanceResponse> GetInstance(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetInstance));

            var sbInstance = await GetInstanceFull(id);
            var instance = new InstanceResponse();
            if (sbInstance != null)
            {
                instance = new InstanceResponse {Id = sbInstance.Id, Name = sbInstance.Name};
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
        public virtual async Task<ServiceBusInstanceModel> GetInstanceFull(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(GetInstanceFull));

            var instance = InstanceRepository
                .GetInstances()
                .FirstOrDefault(x => x.Id == id);
            try
            {
                if (instance != null)
                {
                    _logger.LogInformation("Fetched instance with Id {0}", instance.Id);
                }
                else
                {
                    _logger.LogWarning("Instance with {0} not found", id);
                }

                _logger.LogTrace("Finish {0}", nameof(GetInstanceFull));

                return await Task.FromResult(instance).ConfigureAwait(false);
            }
            catch (ServiceBusException sbe)
            {
                _logger.LogError(sbe, "Error while fetching data from ServiceBus", nameof(GetInstanceFull));

                throw new SbDriverException(sbe.Message)
                {
                    ErrorMessage = new ErrorMessageModel
                    {
                        Code = ErrorConstants.CommunicationsErrorCode,
                        UserMessageText = sbe.Message,
                        SupportReferenceId = new Guid().ToString()
                    }
                };
            }
        }
    }
}