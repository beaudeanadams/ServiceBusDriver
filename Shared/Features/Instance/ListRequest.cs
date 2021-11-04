using System.Collections.Generic;
using MediatR;

namespace ServiceBusDriver.Shared.Features.Instance
{
    public class ListRequest : IRequest<List<InstanceResponseDto>>
    {
    }
}