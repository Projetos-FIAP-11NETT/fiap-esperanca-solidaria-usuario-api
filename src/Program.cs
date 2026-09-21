using FiapEsperancaSolidaria.Usuario.Api.Configurations;
using FiapEsperancaSolidaria.Usuario.Api.Configurations.OpenApi;
using FiapEsperancaSolidaria.Usuario.Application.Configurations;
using FiapEsperancaSolidaria.Usuario.Auth.Configurations;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Configurations;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Correlation;
using FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
using FiapEsperancaSolidaria.Usuario.Queue.Configurations;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddAuthenticationConfig(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

builder.Host.AddSerilogConfig();

builder.Services.AddAuthService(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddQueueConfig(builder.Configuration);

//builder.Services.AddObservabilityConfig();

builder.Services.AddApiConfig(builder.Configuration);

var app = builder.Build();

app.UseApiConfig();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapOpenApiConfiguration();

app.MapHealthCheckEndpoints();

app.ApplyMigrations();

app.Run();
