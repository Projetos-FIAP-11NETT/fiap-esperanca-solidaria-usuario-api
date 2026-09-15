using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.AuthUser;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.CreateUser;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.LogoutSession;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.MakeGestorONG;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Queries.GetSession;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapEsperancaSolidaria.Usuario.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]

public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserCommand command)
    {
        var result = await mediator.Send(command);

        if (result)
            return Created();

        return BadRequest();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserCommand command)
    {
        var result = await mediator.Send(command);

        if (result.IdToken != null)
            return Ok(result);

        return Unauthorized();
    }

    [HttpGet("Session/{sessionId:guid}")]
    public async Task<IActionResult> GetSessionAsync([FromRoute] Guid sessionId)
    {
        var result = await mediator.Send(new GetSessionQuery(sessionId));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("Session/{sessionId:guid}")]
    public async Task<IActionResult> LogoutAsync([FromRoute] Guid sessionId)
    {
        await mediator.Send(new LogoutSessionCommand(sessionId));

        return NoContent();
    }

    [HttpPut("MakeGestorONG")]
    [Authorize(Roles = "GestorONG")]
    public async Task<IActionResult> MakeAdminAsync([FromBody] MakeGestorONGCommand command)
    {
        var result = await mediator.Send(command);

        if (result)
            return Created();

        return BadRequest();
    }
}
