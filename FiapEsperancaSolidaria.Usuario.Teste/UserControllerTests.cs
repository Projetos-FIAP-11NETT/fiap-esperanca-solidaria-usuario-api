using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.AuthUser;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.CreateUser;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.LogoutSession;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.MakeGestorONG;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.UpdateUser;
using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Queries.GetSession;
using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using FiapEsperancaSolidaria.Usuario.Contracts.Requests;
using FiapEsperancaSolidaria.Usuario.Controllers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FiapEsperancaSolidaria.Usuario.Teste;

public class UserControllerTests
{
    [Fact]
    public async Task CreateDoadorAsync_WhenMediatorReturnsTrue_ReturnsCreatedAndSendsDoadorCommand()
    {
        var mediator = new FakeMediator();
        mediator.SetResponse(true);
        var controller = new UserController(mediator);
        var request = new CreateUserRequest("Maria", "maria.doador@email.com", "Senha@123", "12345678901", "image.png");

        var result = await controller.CreateDoadorAsync(request);

        Assert.IsType<CreatedResult>(result);
        var command = Assert.IsType<CreateUserCommand>(mediator.LastRequest);
        Assert.Equal(request.Name, command.Name);
        Assert.Equal(request.Email, command.Email);
        Assert.Equal(request.Password, command.Password);
        Assert.Equal(request.Cpf, command.Cpf);
        Assert.Equal(request.Image, command.Image);
        Assert.False(command.IsGestorONG);
    }

    [Fact]
    public async Task CreateGestorONGAsync_WhenMediatorReturnsTrue_ReturnsCreatedAndSendsGestorCommand()
    {
        var mediator = new FakeMediator();
        mediator.SetResponse(true);
        var controller = new UserController(mediator);
        var request = new CreateUserRequest("Joao", "joao.gestor@email.com", "Senha@123", "12345678901", null);

        var result = await controller.CreateGestorONGAsync(request);

        Assert.IsType<CreatedResult>(result);
        var command = Assert.IsType<CreateUserCommand>(mediator.LastRequest);
        Assert.True(command.IsGestorONG);
    }

    [Fact]
    public async Task CreateDoadorAsync_WhenMediatorReturnsFalse_ReturnsBadRequest()
    {
        var mediator = new FakeMediator();
        mediator.SetResponse(false);
        var controller = new UserController(mediator);
        var request = new CreateUserRequest("Maria", "maria.doador@email.com", "Senha@123", "12345678901", null);

        var result = await controller.CreateDoadorAsync(request);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateDoadorAsync_WhenMediatorReturnsTrue_ReturnsNoContentAndSendsDoadorCommand()
    {
        var userId = Guid.NewGuid();
        var mediator = new FakeMediator();
        mediator.SetResponse(true);
        var controller = new UserController(mediator);
        var request = new UpdateUserRequest("Maria Silva", "maria.silva@email.com", "12345678901", "image.png");

        var result = await controller.UpdateDoadorAsync(userId, request);

        Assert.IsType<NoContentResult>(result);
        var command = Assert.IsType<UpdateUserCommand>(mediator.LastRequest);
        Assert.Equal(userId, command.UserId);
        Assert.Equal(request.Name, command.Name);
        Assert.Equal(request.Email, command.Email);
        Assert.Equal(request.Cpf, command.Cpf);
        Assert.Equal(request.Image, command.Image);
        Assert.False(command.IsGestorONG);
    }

    [Fact]
    public async Task UpdateGestorONGAsync_WhenMediatorReturnsTrue_ReturnsNoContentAndSendsGestorCommand()
    {
        var userId = Guid.NewGuid();
        var mediator = new FakeMediator();
        mediator.SetResponse(true);
        var controller = new UserController(mediator);
        var request = new UpdateUserRequest("Gestor", "gestor@email.com", "12345678901", null);

        var result = await controller.UpdateGestorONGAsync(userId, request);

        Assert.IsType<NoContentResult>(result);
        var command = Assert.IsType<UpdateUserCommand>(mediator.LastRequest);
        Assert.Equal(userId, command.UserId);
        Assert.True(command.IsGestorONG);
    }

    [Fact]
    public async Task UpdateDoadorAsync_WhenMediatorReturnsFalse_ReturnsBadRequest()
    {
        var mediator = new FakeMediator();
        mediator.SetResponse(false);
        var controller = new UserController(mediator);
        var request = new UpdateUserRequest("Maria", "maria.doador@email.com", "12345678901", null);

        var result = await controller.UpdateDoadorAsync(Guid.NewGuid(), request);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task LoginAsync_WhenTokenExists_ReturnsOkWithLoginResponse()
    {
        var response = new LoginResponse
        {
            SessionId = Guid.NewGuid(),
            IdToken = "token",
            RefreshToken = "refresh",
            ExpiresIn = 3600,
            Email = "maria@email.com"
        };
        var mediator = new FakeMediator();
        mediator.SetResponse(response);
        var controller = new UserController(mediator);
        var command = new LoginUserCommand("maria.doador@email.com", "Senha@123");

        var result = await controller.LoginAsync(command);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
        Assert.Same(command, mediator.LastRequest);
    }

    [Fact]
    public async Task LoginAsync_WhenTokenIsNull_ReturnsUnauthorized()
    {
        var mediator = new FakeMediator();
        mediator.SetResponse(new LoginResponse());
        var controller = new UserController(mediator);

        var result = await controller.LoginAsync(new LoginUserCommand("maria.doador@email.com", "Senha@123"));

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetSessionAsync_WhenSessionExists_ReturnsOkWithSession()
    {
        var sessionId = Guid.NewGuid();
        var response = new SessionResponse
        {
            SessionId = sessionId,
            Email = "maria@email.com",
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };
        var mediator = new FakeMediator();
        mediator.SetResponse<SessionResponse?>(response);
        var controller = new UserController(mediator);

        var result = await controller.GetSessionAsync(sessionId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
        var query = Assert.IsType<GetSessionQuery>(mediator.LastRequest);
        Assert.Equal(sessionId, query.SessionId);
    }

    [Fact]
    public async Task GetSessionAsync_WhenSessionDoesNotExist_ReturnsNotFound()
    {
        var mediator = new FakeMediator();
        mediator.SetResponse<SessionResponse?>(null);
        var controller = new UserController(mediator);

        var result = await controller.GetSessionAsync(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task LogoutAsync_SendsLogoutCommandAndReturnsNoContent()
    {
        var sessionId = Guid.NewGuid();
        var mediator = new FakeMediator();
        mediator.SetResponse(true);
        var controller = new UserController(mediator);

        var result = await controller.LogoutAsync(sessionId);

        Assert.IsType<NoContentResult>(result);
        var command = Assert.IsType<LogoutSessionCommand>(mediator.LastRequest);
        Assert.Equal(sessionId, command.SessionId);
    }

    [Fact]
    public async Task MakeAdminAsync_WhenMediatorReturnsTrue_ReturnsCreated()
    {
        var mediator = new FakeMediator();
        mediator.SetResponse(true);
        var controller = new UserController(mediator);
        var command = new MakeGestorONGCommand("gestor@email.com");

        var result = await controller.MakeAdminAsync(command);

        Assert.IsType<CreatedResult>(result);
        Assert.Same(command, mediator.LastRequest);
    }

    [Fact]
    public async Task MakeAdminAsync_WhenMediatorReturnsFalse_ReturnsBadRequest()
    {
        var mediator = new FakeMediator();
        mediator.SetResponse(false);
        var controller = new UserController(mediator);

        var result = await controller.MakeAdminAsync(new MakeGestorONGCommand("gestor@email.com"));

        Assert.IsType<BadRequestResult>(result);
    }

    private sealed class FakeMediator : IMediator
    {
        private readonly Dictionary<Type, object?> _responses = [];

        public object? LastRequest { get; private set; }

        public void SetResponse<TResponse>(TResponse response)
        {
            _responses[typeof(TResponse)] = response;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;

            if (_responses.TryGetValue(typeof(TResponse), out var response))
                return Task.FromResult((TResponse)response!);

            return Task.FromResult(default(TResponse)!);
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            LastRequest = request;
            return Task.CompletedTask;
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(_responses.TryGetValue(request.GetType(), out var response) ? response : null);
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return Task.CompletedTask;
        }
    }
}
