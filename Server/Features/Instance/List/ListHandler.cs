using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using ServiceBusDriver.Db.Repository;
using ServiceBusDriver.Server.Services.AuthContext;
using ServiceBusDriver.Shared.Features.Instance;

namespace ServiceBusDriver.Server.Features.Instance.List
{
    public class ListHandler : IRequestHandler<ListRequest, List<InstanceResponseDto>>
    {
        private readonly IInstanceRepository _instanceRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public ListHandler(ICurrentUser currentUser, IInstanceRepository instanceRepository, IMapper mapper)
        {
            _currentUser = currentUser;
            _instanceRepository = instanceRepository;
            _mapper = mapper;
        }

        public async Task<List<InstanceResponseDto>> Handle(ListRequest request, CancellationToken cancellationToken)
        {
            var entities = await _instanceRepository.GetInstancesWhereAuthUserIdIs(_currentUser.User.Id, cancellationToken);

            var instances = _mapper.Map<List<InstanceResponseDto>>(entities);

            return instances;
        }
    }
}