using Scalar.AspNetCore;

namespace FiapEsperancaSolidaria.Usuario.Api.Configurations.OpenApi;

public static class OpenApiPipeline
{
    public static IEndpointRouteBuilder MapOpenApiConfiguration(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapOpenApi();
        endpoints.MapScalarApiReference();
        return endpoints;
    }
}
