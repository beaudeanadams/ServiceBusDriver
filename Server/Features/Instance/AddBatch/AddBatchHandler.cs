using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusDriver.Core.Components.Connection;
using ServiceBusDriver.Db.Entities;
using ServiceBusDriver.Db.Repository;
using ServiceBusDriver.Server.Services.AuthContext;
using ServiceBusDriver.Server.Services.Password;
using ServiceBusDriver.Shared.Constants;
using ServiceBusDriver.Shared.Features.Error;
using ServiceBusDriver.Shared.Features.Instance;

namespace ServiceBusDriver.Server.Features.Instance.AddBatch
{
    public class AddBatchHandler : IRequestHandler<AddBatchRequestDto, List<InstanceResponseDto>>
    {
        private readonly IConnectionService _connectionService;
        private readonly ICurrentUser _currentUser;
        private readonly IAesEncryptService _aesEncryptService;
        private readonly IInstanceRepository _instanceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddBatchHandler> _logger;

        public AddBatchHandler(ILogger<AddBatchHandler> logger,
            IMapper mapper, ICurrentUser currentUser,
            IConnectionService connectionService,
            IAesEncryptService aesEncryptService,
            IInstanceRepository instanceRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _currentUser = currentUser;
            _connectionService = connectionService;
            _aesEncryptService = aesEncryptService;
            _instanceRepository = instanceRepository;
        }

        public async Task<List<InstanceResponseDto>> Handle(AddBatchRequestDto requestDto, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            if (_currentUser.User.Email.EndsWith("test.com"))
            {
                throw new AppException()
                {
                    HttpStatusCode = StatusCodes.Status400BadRequest,
                    ErrorMessage = new AppErrorMessageDto
                    {
                        Code = AppErrorConstants.BadRequestErrorCode,
                        UserMessageText = "Adding instance not allowed for Test Accounts"
                    }
                }; 
            }
            
            var responseList = new List<InstanceResponseDto>();

            foreach (var connectionString in requestDto.ConnectionStrings)
            {
                try
                {
                    var sb = await _connectionService.ProcessConnectionString(connectionString);

                    if (sb.TransportType != ServiceBusTransportType.AmqpTcp)
                    {
                        _logger.LogError("Cannot add to Db as Only Tcp Connections are supported now - {0}", sb.Namespace);
                        continue;
                    }


                    var instanceEntity = new InstanceEntity();
                    instanceEntity.RawConnectionString = _aesEncryptService.Encrypt(connectionString);
                    instanceEntity.Uri = sb.Uri;
                    instanceEntity.Namespace = sb.Namespace;
                    instanceEntity.ServicePath = sb.ServicePath;
                    instanceEntity.TransportType = sb.TransportType.ToString();
                    instanceEntity.StsEndpoint = sb.StsEndpoint;
                    instanceEntity.RuntimePort = sb.RuntimePort;
                    instanceEntity.ManagementPort = sb.ManagementPort;
                    instanceEntity.SharedAccessKeyName = sb.SharedAccessKeyName;
                    instanceEntity.SharedAccessKey = _aesEncryptService.Encrypt(sb.SharedAccessKey);
                    instanceEntity.EntityPath = sb.EntityPath;
                    instanceEntity.UserId = _currentUser.User.Id;

                    var checkExistingInstance = await _instanceRepository.GetInstanceWhereNamespaceIs(_currentUser.User.Id, instanceEntity.Namespace, cancellationToken);

                    if (checkExistingInstance.Count > 0)
                    {
                        _logger.LogError("Cannot add to Db as User already has same instance added - {0}", sb.Namespace);
                        continue;
                    }

                    instanceEntity = await _instanceRepository.Add(instanceEntity, cancellationToken);

                    _logger.LogInformation(JsonConvert.SerializeObject(instanceEntity));

                    var response = _mapper.Map<InstanceResponseDto>(instanceEntity);

                    responseList.Add(response);
                }
                catch (Exception e) when (!(e is AppException))
                {
                    _logger.LogError("Cannot add to Db as Connectivity Failed - {0}", connectionString);
                }
            }

            return responseList;
        }
    }
}