using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDriver.Shared.Features.Subscription;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Server.Features.Subscription
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubscriptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("get")]
        [Produces("application/json")]
        public async Task<ActionResult> Get([FromQuery] GetRequest request)
        {
            Guarantee.NotNull(request);
            Guarantee.NotNull(request.SubscriptionName);
            Guarantee.NotNull(request.InstanceId);
            Guarantee.NotNull(request.TopicName);

            var result = await _mediator.Send(request);

            return Ok(result);
        }

        [HttpPost]
        [Route("purge")]
        [Produces("application/json")]
        public async Task<ActionResult> Purge([FromBody] PurgeRequest request)
        {
            Guarantee.NotNull(request);
            Guarantee.NotNull(request.SubscriptionName);
            Guarantee.NotNull(request.InstanceId);
            Guarantee.NotNull(request.TopicName);

            var result = await _mediator.Send(request);

            return Ok(result);
        }


    }
}