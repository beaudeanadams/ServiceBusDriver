using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDriver.Shared.Features.Message;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Server.Features.Message
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("search")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> Search([FromBody] SearchRequest request)
        {
            Guarantee.NotNull(request);
            Guarantee.NotNull(request.SubscriptionName);
            Guarantee.NotNull(request.InstanceId);
            Guarantee.NotNull(request.TopicName);
            Guarantee.NotNull(request.Value);

            var result = await _mediator.Send(request);

            return Ok(result);
        }
    }
}