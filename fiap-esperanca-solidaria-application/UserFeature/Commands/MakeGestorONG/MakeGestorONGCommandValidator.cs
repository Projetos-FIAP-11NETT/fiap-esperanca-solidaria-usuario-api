using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.MakeGestorONG;

public class MakeGestorONGCommandValidator : AbstractValidator<MakeGestorONGCommand>
{
    public MakeGestorONGCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail não pode ser vazio.")
            .MaximumLength(255).WithMessage("O e-mail deve conter no máximo 255 caracteres.")
            .EmailAddress().WithMessage("O e-mail deve conter um formato válido.");
    }
}
