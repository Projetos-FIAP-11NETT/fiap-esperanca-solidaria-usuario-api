using FluentValidation;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator
    : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("O usuário não pode ser vazio.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome não pode ser vazio.")
            .MaximumLength(50).WithMessage("O nome deve conter no máximo 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail não pode ser vazio.")
            .MaximumLength(255).WithMessage("O e-mail deve conter no máximo 255 caracteres.")
            .EmailAddress().WithMessage("O e-mail deve conter um formato válido.");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("O CPF não pode ser vazio.")
            .Length(11).WithMessage("O CPF deve conter 11 caracteres.")
            .Must(cpf => cpf is not null && cpf.All(char.IsDigit)).WithMessage("O CPF deve conter apenas números.");
    }
}
