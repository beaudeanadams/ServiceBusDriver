using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDriver.Shared.Features.Instance;

namespace ServiceBusDriver.Server.Features.Instance
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InstanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InstanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("list")]
        [Produces("application/json")]
        public async Task<ActionResult> List()
        {
            var result = await _mediator.Send(new ListRequest());

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> Get(string id)
        {
            var result = await _mediator.Send(new GetRequestDto(id));

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}/topics")]
        [Produces("application/json")]
        public async Task<ActionResult> GetAllTopics([FromRoute] string id)
        {
            var result = await _mediator.Send(new GetTopicsRequest(id));

            return Ok(result);
        }
        
        [HttpGet]
        [Route("{id}/queues")]
        [Produces("application/json")]
        public async Task<ActionResult> GetAllQueue([FromRoute] string id)
        {
            var result = await _mediator.Send(new GetQueuesRequest(id));

            return Ok(result);
        }

        [HttpPost]
        [Route("")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> Add([FromBody] AddRequestDto request)
        {
            var result = await _mediator.Send(request);

            return Ok(result);
        }

        [HttpPost]
        [Route("batch")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> AddBatch([FromBody] AddBatchRequestDto request)
        {
            var result = await _mediator.Send(request);

            return Ok(result);
        }
    }
}