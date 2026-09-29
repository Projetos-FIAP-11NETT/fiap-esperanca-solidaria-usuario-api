using FiapEsperancaSolidaria.Usuario.Api.Configurations;
using FiapEsperancaSolidaria.Usuario.Api.Configurations.OpenApi;
using FiapEsperancaSolidaria.Usuario.Application.Configurations;
using FiapEsperancaSolidaria.Usuario.Auth.Configurations;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Configurations;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Correlation;
using FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
using FiapEsperancaSolidaria.Usuario.Queue.Configurations;
using FiapEsperancaSolidaria.Usuario.Observability.Configurations;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddAuthenticationConfig(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Host.AddSerilogConfig();

builder.Services.AddAuthService(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddQueueConfig(builder.Configuration);

builder.Services.AddObservability(builder.Configuration);

builder.Services.AddApiConfig(builder.Configuration);

var app = builder.Build();

app.UseApiConfig();

if (!app.Environment.IsEnvironment("Kubernetes"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapOpenApiConfiguration();
app.MapObservabilityEndpoints();

app.MapHealthCheckEndpoints();

app.ApplyMigrations();

app.Run();
