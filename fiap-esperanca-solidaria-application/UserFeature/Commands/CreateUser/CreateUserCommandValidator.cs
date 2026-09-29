using FluentValidation;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.CreateUser;

public sealed class CreateUserCommandValidator
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome não pode ser vazio.")
            .MaximumLength(50).WithMessage("O nome deve conter no máximo 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail não pode ser vazio.")
            .MaximumLength(255).WithMessage("O e-mail deve conter no máximo 255 caracteres.")
            .EmailAddress().WithMessage("O e-mail deve conter um formato válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha não pode ser vazia.")
            .MinimumLength(8).WithMessage("A senha deve conter ao menos 8 caracteres.")
            .Must(ContainLetter).WithMessage("A senha deve conter ao menos uma letra.")
            .Must(ContainNumber).WithMessage("A senha deve conter ao menos um número.")
            .Must(ContainSpecialChar).WithMessage("A senha deve conter ao menos um caractere especial.");
    }

    private static bool ContainLetter(string password)
        => password.Any(char.IsLetter);

    private static bool ContainNumber(string password)
        => password.Any(char.IsDigit);

    private static bool ContainSpecialChar(string password)
        => password.Any(c => !char.IsLetterOrDigit(c));
}