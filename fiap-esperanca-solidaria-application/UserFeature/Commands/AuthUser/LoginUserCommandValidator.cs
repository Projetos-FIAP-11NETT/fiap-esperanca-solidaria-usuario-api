using FluentValidation;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.AuthUser;

public sealed class LoginUserCommandValidator
    : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha não pode ser vazia.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail não pode ser vazio.")
            .EmailAddress().WithMessage("O e-mail deve conter um formato válido.");
    }
}