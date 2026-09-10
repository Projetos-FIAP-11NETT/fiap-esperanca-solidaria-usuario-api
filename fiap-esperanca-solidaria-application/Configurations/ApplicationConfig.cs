using Microsoft.Extensions.DependencyInjection;

namespace FiapEsperancaSolidaria.Usuario.Application.Configurations;

public static class ApplicationConfig
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatorConfig();
        services.AddValidatorConfig();

        return services;
    }
}