using FiapEsperancaSolidaria.Usuario.Auth;
using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories;
using FiapEsperancaSolidaria.Usuario.Domain.Exceptions;
using FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.UpdateUser;

public class UpdateUserCommandHandler(
        IAuthService authService,
        ICurrentUserAccessor currentUserAccessor,
        IUserRepository userRepository
    )
    : IRequestHandler<UpdateUserCommand, bool>
{
    public async Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (currentUserAccessor.UserId != command.UserId)
            throw new ForbiddenException("Você não tem permissão para alterar esse usuário.");

        var user = await userRepository.GetByIdWithRolesAsync(command.UserId)
            ?? throw new NotFoundException("Usuário não encontrado.");

        var expectedRoleName = command.IsGestorONG ? "GestorONG" : "Doador";
        var hasExpectedRole = user.Roles.Any(role => role.Name == expectedRoleName);

        if (!hasExpectedRole)
            throw new BusinessException($"Usuário não possui o perfil {expectedRoleName}.");

        var emailChanged = !string.Equals(user.Email, command.Email, StringComparison.OrdinalIgnoreCase);
        if (emailChanged && await userRepository.ExistsEmailAsync(command.Email))
            throw new BusinessException("Esse e-mail já está cadastrado");

        user.UpdateProfile(command.Name, command.Email, command.Cpf, command.Image);

        if (string.IsNullOrWhiteSpace(user.FirebaseUserId))
            throw new BusinessException("Usuário não possui vínculo com o serviço de autenticação.");

        await authService.UpdateUserAsync(user.FirebaseUserId, user.Email, user.Name);

        userRepository.Update(user);
        return await userRepository.SaveChangesAsync(cancellationToken);
    }
}
