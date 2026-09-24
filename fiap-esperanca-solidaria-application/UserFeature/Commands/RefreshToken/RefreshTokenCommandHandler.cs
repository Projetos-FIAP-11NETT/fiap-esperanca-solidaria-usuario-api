using FiapEsperancaSolidaria.Usuario.Application.Sessions;
using FiapEsperancaSolidaria.Usuario.Auth;
using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IAuthService authService,
    ISessionCacheService sessionCacheService,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var session = await sessionCacheService.GetAsync(command.SessionId, cancellationToken);

        if (session is null || session.RefreshToken != command.RefreshToken)
            throw new UnauthorizedAccessException("Sessão ou refresh token inválido");

        var refreshedToken = await authService.RefreshTokenAsync(command.RefreshToken);
        var now = DateTimeOffset.UtcNow;
        var sessionExpiresAt = now.Add(SessionLifetime.Duration);

        session.IdToken = refreshedToken.IdToken;
        session.RefreshToken = refreshedToken.RefreshToken;
        session.TokenExpiresAt = now.AddSeconds(refreshedToken.ExpiresIn);
        session.SessionExpiresAt = sessionExpiresAt;
        session.ExpiresAt = sessionExpiresAt;

        await sessionCacheService.StoreAsync(
            session,
            SessionLifetime.Duration,
            cancellationToken);

        logger.LogInformation("Session {SessionId} refreshed for user {Email}", session.SessionId, session.Email);

        return new LoginResponse
        {
            SessionId = session.SessionId,
            IdToken = session.IdToken,
            RefreshToken = session.RefreshToken,
            ExpiresIn = refreshedToken.ExpiresIn,
            Email = session.Email
        };
    }
}
