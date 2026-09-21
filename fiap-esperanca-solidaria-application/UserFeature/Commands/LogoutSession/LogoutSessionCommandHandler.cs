using FiapEsperancaSolidaria.Usuario.Application.Sessions;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.LogoutSession;

public sealed class LogoutSessionCommandHandler(ISessionCacheService sessionCacheService)
    : IRequestHandler<LogoutSessionCommand, bool>
{
    public async Task<bool> Handle(LogoutSessionCommand command, CancellationToken cancellationToken)
    {
        await sessionCacheService.RemoveAsync(command.SessionId, cancellationToken);
        return true;
    }
}