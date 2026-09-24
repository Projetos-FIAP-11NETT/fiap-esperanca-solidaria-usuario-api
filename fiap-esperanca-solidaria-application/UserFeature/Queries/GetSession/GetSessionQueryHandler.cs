using FiapEsperancaSolidaria.Usuario.Application.Sessions;
using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Queries.GetSession;

public sealed class GetSessionQueryHandler(ISessionCacheService sessionCacheService)
    : IRequestHandler<GetSessionQuery, SessionResponse?>
{
    public async Task<SessionResponse?> Handle(GetSessionQuery query, CancellationToken cancellationToken)
    {
        var session = await sessionCacheService.GetAsync(query.SessionId, cancellationToken);

        if (session is null)
            return null;

        return new SessionResponse
        {
            SessionId = session.SessionId,
            Email = session.Email,
            CreatedAt = session.CreatedAt,
            ExpiresAt = session.ExpiresAt,
            TokenExpiresAt = session.TokenExpiresAt,
            SessionExpiresAt = session.SessionExpiresAt
        };
    }
}
