using FiapEsperancaSolidaria.Usuario.Shared.Option;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FiapEsperancaSolidaria.Usuario.Api.Configurations;

public static class HealthCheckConfiguration
{
    private static readonly string[] tags = ["db", "postgres", "ready"];


    public static IServiceCollection AddHealthCheckConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetSection("ConnectionStrings")
            .Get<ConnectionStringsOptions>();

        services.AddHealthChecks()
            .AddNpgSql(
                connectionString.DefaultConnection,
                name: "users-db",
                failureStatus: HealthStatus.Unhealthy,
                tags: tags);

        return services;
    }

    public static IEndpointRouteBuilder MapHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health");
        endpoints.MapHealthChecks("/health/ready");
        endpoints.MapHealthChecks("/health/live");

        return endpoints;
    }
}
