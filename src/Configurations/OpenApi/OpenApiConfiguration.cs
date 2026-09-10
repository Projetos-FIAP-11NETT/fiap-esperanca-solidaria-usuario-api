using Microsoft.OpenApi;

namespace FiapEsperancaSolidaria.Usuario.Api.Configurations.OpenApi;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "FIAP Esperança Solidária - Usuário API",
                    Version = "v1",
                    Description = "API de Criação e login de usuários para a plataforma FIAP Esperança Solidária",
                    Contact = new OpenApiContact
                    {
                        Name = "FIAP Esperança Solidária Team",
                        Email = "contato@fiapesperancasolidaria.com"
                    }
                };

                return Task.CompletedTask;
            });
        });

        return services;
    }
}
