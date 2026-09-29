using FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.CreateUser;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FiapEsperancaSolidaria.Usuario.Application.Configurations;

public static class ValidatorConfig
{
    public static void AddValidatorConfig(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
    }
}
