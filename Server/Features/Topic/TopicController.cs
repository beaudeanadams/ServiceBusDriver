using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDriver.Shared.Features.Topic;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Server.Features.Topic
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TopicController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("all")]
        [Produces("application/json")]
        public async Task<ActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllRequest());

            return Ok(result);
        }

        [HttpGet]
        [Route("get")]
        [Produces("application/json")]
        public async Task<ActionResult> Get([FromQuery] GetRequest request)
        {
            Guarantee.NotNull(request);
            Guarantee.NotNull(request.InstanceId);
            Guarantee.NotNull(request.TopicName);

            var result = await _mediator.Send(request);

            return Ok(result);
        }

        [HttpGet]
        [Route("subscriptions")]
        [Produces("application/json")]
        public async Task<ActionResult> GetSubscriptions([FromQuery] GetSubscriptionsRequest request)
        {
            Guarantee.NotNull(request);
            Guarantee.NotNull(request.InstanceId);
            Guarantee.NotNull(request.TopicName);

            var result = await _mediator.Send(request);

            return Ok(result);
        }
    }
}