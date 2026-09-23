using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string Name,
    string Email,
    string Cpf,
    string? Image,
    bool IsGestorONG) : IRequest<bool>;
