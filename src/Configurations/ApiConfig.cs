using FiapEsperancaSolidaria.Usuario.Api.Configurations.OpenApi;
using FiapEsperancaSolidaria.Usuario.Api.Middlewares;
using FiapEsperancaSolidaria.Usuario.Middlewares;
using FiapEsperancaSolidaria.Usuario.Observability.Middleware;


namespace FiapEsperancaSolidaria.Usuario.Api.Configurations;

public static class ApiConfig
{
    private const string FrontendCorsPolicy = "Frontend";

    public static IServiceCollection AddApiConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddOpenApiConfiguration();

        // Sem CORS o navegador bloqueia login/cadastro chamados pelo campanha-web.
        // As origens vêm de Cors:AllowedOrigins (mesmo padrão da campanha-api); sem
        // configuração nenhuma origem é liberada.
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendCorsPolicy, policy =>
                policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod());
        });

        return services;
    }

    public static void UseApiConfig(this WebApplication app)
    {
        app.UseCors(FrontendCorsPolicy);
        //app.UseMiddleware<ObservabilityMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
