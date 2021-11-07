using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDriver.Shared.Features.Queue;
using ServiceBusDriver.Shared.Tools;
using System.Threading.Tasks;

namespace ServiceBusDriver.Server.Features.Queue
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QueueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("get")]
        [Produces("application/json")]
        public async Task<ActionResult> Get([FromQuery] GetRequest request)
        {
            Guarantee.NotNull(request);
            Guarantee.NotNull(request.InstanceId);
            Guarantee.NotNull(request.QueueName);

            var result = await _mediator.Send(request);

            return Ok(result);
        }
    }
}