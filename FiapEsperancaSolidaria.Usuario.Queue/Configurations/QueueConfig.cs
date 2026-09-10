using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Publisher;
using FiapEsperancaSolidaria.Usuario.Queue.Configurations.MassTransit;
using FiapEsperancaSolidaria.Usuario.Queue.Configurations.Sqs;
using FiapEsperancaSolidaria.Usuario.Queue.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace FiapEsperancaSolidaria.Usuario.Queue.Configurations;

public static class QueueConfig
{
    public static IServiceCollection AddQueueConfig(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<MassTransitSettings>(configuration.GetSection(nameof(MassTransitSettings)));
        services.AddScoped<IEmailNotificationPublisher, EmailNotificationPublisher>();

        // AWS SQS
        services.Configure<SqsSettings>(configuration.GetSection(nameof(SqsSettings)));
        services.RegisterSqsStartup();
        return services;
    }
}