using FiapEsperancaSolidaria.Usuario.Api.Configurations.OpenApi;
using FiapEsperancaSolidaria.Usuario.Api.Middlewares;
using FiapEsperancaSolidaria.Usuario.Middlewares;
using FiapEsperancaSolidaria.Usuario.Observability.Middleware;


namespace FiapEsperancaSolidaria.Usuario.Api.Configurations;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddOpenApiConfiguration();

        return services;
    }

    public static void UseApiConfig(this WebApplication app)
    {
        app.UseMiddleware<ObservabilityMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
