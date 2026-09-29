using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.MakeGestorONG;

public class MakeGestorONGCommand : IRequest<bool>
{
    public string Email { get; init; }

    public MakeGestorONGCommand(string email)
    {
        Email = email;
    }
}
