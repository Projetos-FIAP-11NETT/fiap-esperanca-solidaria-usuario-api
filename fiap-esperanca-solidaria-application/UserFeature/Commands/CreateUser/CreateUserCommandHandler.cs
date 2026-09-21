using FiapEsperancaSolidaria.Usuario.Auth;
using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Publisher;
using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories;
using FiapEsperancaSolidaria.Usuario.Domain.Entities;
using FiapEsperancaSolidaria.Usuario.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.CreateUser;

public class CreateUserCommandHandler
    (
        IAuthService authService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IEmailNotificationPublisher emailNotificationPublisher,
        ILogger<CreateUserCommandHandler> logger
    )
    : IRequestHandler<CreateUserCommand, bool>
{

    public async Task<bool> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var emailAlreadyExists = await userRepository.ExistsEmailAsync(command.Email);
        if (emailAlreadyExists)
            throw new BusinessException("Esse e-mail já está cadastrado");

        var role = await roleRepository.FindDoadorRoleAsync();
        if (role is null)
            throw new NotFoundException("Perfil de usuário não encontrado");

        var user = new User(command.Name, command.Email, command.Password, command.Cpf, command.Image, role);

        var firebaseUserId = await CreateUserInFirebase(user);
        if (string.IsNullOrEmpty(firebaseUserId))
            throw new ExternalException("Firebase", "Não foi possível criar a conta no Firebase");

        user.SetFirebaseUserId(firebaseUserId);
        await userRepository.AddAsync(user);

        var result = await userRepository.SaveChangesAsync(cancellationToken);

        //if (result)
        //{
        //    try
        //    {
        //        await emailNotificationPublisher.PublishAsync(user.Email, "Bem-vindo ao FiapCloudGames!", $"Olá {user.Name}, bem-vindo ao FiapCloudGames😎! Acesse a nossa biblioteca com os nossos novos jogos de sucesso!!");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "Erro ao publicar evento de usuário criado para o usuário {UserId}", user.Id);
        //    }
        //}
        return result;
    }


    private async Task<string> CreateUserInFirebase(User user)
    {
        var firebaseUserId = await authService.CreateUserAsync(
            user.Email,
            user.Password,
            user.Name,
            user.Roles.Select(x => x.Name),
            user.Id);

        return firebaseUserId;
    }
}
