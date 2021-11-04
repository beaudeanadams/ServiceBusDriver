using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDriver.Shared.Models;

namespace ServiceBusDriver.Server.Features.Connection
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConnectionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("process")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> ProcessConnectionString([FromBody] ProcessConnectionStringRequest request)
        {
            var result = await _mediator.Send(request);

            return Ok(result);
        }

        [HttpPost]
        [Route("test")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> TestConnection([FromBody] TestConnectionRequest request)
        {
            var result = await _mediator.Send(request);

            return Ok(result);
        }
    }
}