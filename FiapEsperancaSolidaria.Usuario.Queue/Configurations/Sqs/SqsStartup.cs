using Amazon.SimpleNotificationService;
using Amazon.SQS;
using FiapEsperancaSolidaria.Usuario.Queue.Configurations.MassTransit;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FiapEsperancaSolidaria.Usuario.Queue.Configurations.Sqs;

public static class SqsStartup
{
    public static void RegisterSqsStartup(this IServiceCollection services)
    {
        services.AddMassTransit<ISqsPublish>(x =>
        {
            x.SetEndpointNameFormatter(
                new KebabCaseEndpointNameFormatter("users", false));

            x.UsingAmazonSqs((context, cfg) =>
            {
                var sqsSettings = context.GetRequiredService<IOptions<SqsSettings>>().Value;
                var massTransitSettings = context.GetRequiredService<IOptions<MassTransitSettings>>().Value;

                cfg.Host(sqsSettings.Region, h =>
                {
                    h.AccessKey(sqsSettings.AccessKey);
                    h.SecretKey(sqsSettings.SecretKey);

                    if (!string.IsNullOrWhiteSpace(sqsSettings.ServiceUrl))
                    {
                        h.Config(new AmazonSQSConfig
                        {
                            ServiceURL = sqsSettings.ServiceUrl,
                            AuthenticationRegion = sqsSettings.Region
                        });

                        h.Config(new AmazonSimpleNotificationServiceConfig
                        {
                            ServiceURL = sqsSettings.ServiceUrl,
                            AuthenticationRegion = sqsSettings.Region
                        });
                    }
                });

                cfg.UseMessageRetry(r => r.Interval(massTransitSettings.RetryCount, massTransitSettings.Interval));

                //cfg.UseConsumeFilter(typeof(NewRelicConsumeFilter<>), context);
                //cfg.UsePublishFilter(typeof(NewRelicPublishFilter<>), context);

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddSingleton<IAmazonSQS>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<SqsSettings>>().Value;
            return new AmazonSQSClient(settings.AccessKey, settings.SecretKey, new AmazonSQSConfig
            {
                ServiceURL = settings.ServiceUrl,
                AuthenticationRegion = settings.Region
            });
        });
    }
}
