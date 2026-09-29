using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FiapEsperancaSolidaria.Usuario.Auth.Configurations.Firebase;

namespace FiapEsperancaSolidaria.Usuario.Auth.Configurations;

public static class AuthServiceConfig
{
    public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFirebase(configuration);

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
