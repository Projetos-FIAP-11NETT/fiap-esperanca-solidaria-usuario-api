using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.AuthUser;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.CreateUser;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.LogoutSession;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.MakeGestorONG;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.RefreshToken;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.UpdateUser;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.UploadUserImage;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Queries.GetSession;
using FiapEsperancaSolidaria.Usuario.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapEsperancaSolidaria.Usuario.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]

public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost("Doador")]
    public async Task<IActionResult> CreateDoadorAsync([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.Name,
            request.Email,
            request.Password,
            request.Cpf,
            request.Image,
            isGestorONG: false);

        var result = await mediator.Send(command);

        if (result)
            return Created();

        return BadRequest();
    }

    [HttpPost("GestorONG")]
    [Authorize(Roles = "GestorONG")]
    public async Task<IActionResult> CreateGestorONGAsync([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.Name,
            request.Email,
            request.Password,
            request.Cpf,
            request.Image,
            isGestorONG: true);

        var result = await mediator.Send(command);

        if (result)
            return Created();

        return BadRequest();
    }

    [HttpPut("Doador/{userId:guid}")]
    [Authorize(Roles = "Doador")]
    public async Task<IActionResult> UpdateDoadorAsync([FromRoute] Guid userId, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(
            userId,
            request.Name,
            request.Email,
            request.Cpf,
            request.Image,
            IsGestorONG: false);

        var result = await mediator.Send(command);

        if (result)
            return NoContent();

        return BadRequest();
    }

    [HttpPut("GestorONG/{userId:guid}")]
    [Authorize(Roles = "GestorONG")]
    public async Task<IActionResult> UpdateGestorONGAsync([FromRoute] Guid userId, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(
            userId,
            request.Name,
            request.Email,
            request.Cpf,
            request.Image,
            IsGestorONG: true);

        var result = await mediator.Send(command);

        if (result)
            return NoContent();

        return BadRequest();
    }

    /// <summary>Sobe uma foto de perfil pro S3 (LocalStack em dev) e devolve a URL pública.</summary>
    /// <remarks>
    /// Anônimo de propósito: no cadastro (POST /User/Doador ou /User/GestorONG) ainda não
    /// existe conta pra autenticar contra — o front sobe a imagem primeiro e manda a URL de
    /// volta como o campo Image do cadastro, igual ao fluxo de imagem de campanha na
    /// campanha-api (lá autenticado, aqui não tem como). Mitigação: só tamanho (5 MB) e
    /// content-type de imagem, sem outra validação.
    /// </remarks>
    [HttpPost("images")]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> UploadImageAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "Arquivo vazio." });

        if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { error = "Arquivo precisa ser uma imagem." });

        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(
            new UploadUserImageCommand(stream, file.FileName, file.ContentType),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserCommand command)
    {
        var result = await mediator.Send(command);

        if (!string.IsNullOrWhiteSpace(result.IdToken))
            return Ok(result);

        return Unauthorized();
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenCommand command)
    {
        var result = await mediator.Send(command);

        if (!string.IsNullOrWhiteSpace(result.IdToken))
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
