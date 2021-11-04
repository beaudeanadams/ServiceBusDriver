using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Server.Features.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> Get(string id)
        {
            var result = await _mediator.Send(new GetRequestDto(id));

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        [Route("me")]
        [Produces("application/json")]
        public async Task<ActionResult> GetMe()
        {
            var result = await _mediator.Send(new GetMeRequestDto());

            return Ok(result);
        }

        [HttpPost]
        [Route("")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> Add([FromBody] AddRequestDto requestDto)
        {
            var result = await _mediator.Send(requestDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _mediator.Send(request);

            return Ok(result);
        }

        [HttpPost]
        [Route("verify")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> Verify([FromBody] VerifyOtpRequest request)
        {
            var result = await _mediator.Send(request);

            return Ok(result);
        }
    }
}