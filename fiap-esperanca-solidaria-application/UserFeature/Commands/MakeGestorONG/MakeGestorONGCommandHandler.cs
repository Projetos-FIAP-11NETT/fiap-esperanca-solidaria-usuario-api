using FiapEsperancaSolidaria.Usuario.Auth;
using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories;
using FiapEsperancaSolidaria.Usuario.Domain.Exceptions;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.MakeGestorONG;

public class MakeGestorONGCommandHandler(
        IAuthService authService,
        IUserRepository userRepository,
        IRoleRepository roleRepository
    )
    : IRequestHandler<MakeGestorONGCommand, bool>
{
    public async Task<bool> Handle(MakeGestorONGCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(command.Email)
            ?? throw new NotFoundException("Operação não realizada. O usuário não encontrado.");

        if (user is null)
            return false;

        var role = await roleRepository.FindGestorRoleAsync();
        user.MakeAdmin(role);

        try
        {
            await authService.SetUserRoleAsync(user.FirebaseUserId, user.Roles.Select(x => x.Name), user.Id);
        }
        catch (Exception ex)
        {
            throw new ExternalException("Firebase", "Operação não realizada. Perfil não atualizado no serviço de autenticação.");
        }

        try
        {
            userRepository.Update(user);
            return await userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw new DomainException("Operação não realizada. Perfil não atualizado no banco de dados.");
        }
    }
}