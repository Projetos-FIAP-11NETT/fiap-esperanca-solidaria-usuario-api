using FluentValidation;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
