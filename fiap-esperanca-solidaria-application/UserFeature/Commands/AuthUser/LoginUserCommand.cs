using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.AuthUser;

public sealed record class LoginUserCommand
(
    string Email,
    string Password
)
    : IRequest<LoginResponse>;