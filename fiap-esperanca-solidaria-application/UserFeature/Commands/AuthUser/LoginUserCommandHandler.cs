using FiapEsperancaSolidaria.Usuario.Application.Sessions;
using FiapEsperancaSolidaria.Usuario.Auth;
using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.AuthUser;

public class LoginUserCommandHandler
    (
        IAuthService firebaseAuthService,
        ISessionCacheService sessionCacheService,
        ILogger<LoginUserCommandHandler> logger
    )
    : IRequestHandler<LoginUserCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var responseToken = await AuthUserInFirebase(command);
        var sessionId = Guid.NewGuid();
        var expiration = TimeSpan.FromSeconds(responseToken.ExpiresIn);
        var now = DateTimeOffset.UtcNow;

        await sessionCacheService.StoreAsync(
            new SessionCacheEntry
            {
                SessionId = sessionId,
                Email = responseToken.Email,
                IdToken = responseToken.IdToken,
                RefreshToken = responseToken.RefreshToken,
                CreatedAt = now,
                ExpiresAt = now.Add(expiration)
            },
            expiration,
            cancellationToken);

        logger.LogInformation("Session {SessionId} stored in distributed cache for user {Email}", sessionId, responseToken.Email);

        return new LoginResponse
        {
            SessionId = sessionId,
            IdToken = responseToken.IdToken,
            RefreshToken = responseToken.RefreshToken,
            ExpiresIn = responseToken.ExpiresIn,
            Email = responseToken.Email
        };
    }

    private async Task<LoginResponse> AuthUserInFirebase(LoginUserCommand command)
    {
        var responseToken = await firebaseAuthService.LoginUserAsync(
            command.Email,
            command.Password);
        return responseToken;
    }
}
