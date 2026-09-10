using FiapEsperancaSolidaria.Usuario.Application.Common;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.CreateUser;

public sealed record CreateUserCommand : IRequest<bool>
{
    public string Name { get; init; }
    public string Email { get; init; }

    [SensitiveData]
    public string Password { get; init; }

    public CreateUserCommand(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
}
