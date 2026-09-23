using FiapEsperancaSolidaria.Usuario.Application.Common;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.CreateUser;

public sealed record CreateUserCommand : IRequest<bool>
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string Cpf { get; set; }
    public string? Image { get; private set; }

    [SensitiveData]
    public string Password { get; init; }

    public bool IsGestorONG { get; init; }

    public CreateUserCommand(string name, string email, string password, string cpf, string? image, bool isGestorONG)
    {
        Name = name;
        Email = email;
        Password = password;
        Cpf = cpf;
        Image = image;
        IsGestorONG = isGestorONG;
    }
}
