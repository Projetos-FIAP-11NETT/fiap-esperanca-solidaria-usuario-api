using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.RefreshToken;

public sealed record class RefreshTokenCommand(Guid SessionId, string RefreshToken) : IRequest<LoginResponse>;
