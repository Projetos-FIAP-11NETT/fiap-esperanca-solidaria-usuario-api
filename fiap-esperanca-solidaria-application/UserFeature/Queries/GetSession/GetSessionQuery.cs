using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Queries.GetSession;

public sealed record class GetSessionQuery(Guid SessionId) : IRequest<SessionResponse?>;
