using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDriver.Shared.Features.Message;
using ServiceBusDriver.Shared.Tools;
using System.Threading.Tasks;

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
            Guarantee.NotNull(request.InstanceId);
            Guarantee.NotNull(request.Value);

            var result = await _mediator.Send(request);

            return Ok(result);
        }

        [HttpGet]
        [Route("active")]
        [Produces("application/json")]
        public async Task<ActionResult> GetActiveMessages([FromQuery] GetActiveMessagesRequest request)
        {

            Guarantee.NotNull(request);
            Guarantee.NotNull(request.InstanceId);

            var result = await _mediator.Send(request);

            return Ok(result);
        }

        [HttpGet]
        [Route("last/{limit}")]
        [Produces("application/json")]
        public async Task<ActionResult> GetLastNMessages([FromQuery] GetLastNMessages request, [FromRoute] int limit)
        {
            Guarantee.NotNull(request);
            Guarantee.NotNull(request.InstanceId);

            request.Limit = limit;
            var result = await _mediator.Send(request);

            return Ok(result);
        }


        [HttpGet]
        [Route("deadletter")]
        [Produces("application/json")]
        public async Task<ActionResult> GetDeadLetterMessages([FromQuery] GetDeadLetteredMessagesRequest request)
        {

            Guarantee.NotNull(request);
            Guarantee.NotNull(request.InstanceId);

            var result = await _mediator.Send(request);

            return Ok(result);
        }
    }
}