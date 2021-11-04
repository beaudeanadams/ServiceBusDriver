using System.Collections.Generic;
using MediatR;

namespace ServiceBusDriver.Shared.Features.Instance
{
    public class AddBatchRequestDto : IRequest<List<InstanceResponseDto>>
    {
        public string[] ConnectionStrings { get; set; }
    }
}