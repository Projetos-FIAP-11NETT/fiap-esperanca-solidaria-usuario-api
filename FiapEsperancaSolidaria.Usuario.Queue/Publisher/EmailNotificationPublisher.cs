using Amazon.SQS;
using Amazon.SQS.Model;
using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Publisher;
using FiapEsperancaSolidaria.Usuario.Queue.Configurations.Sqs;
using FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FiapEsperancaSolidaria.Usuario.Queue.Publisher;

public class EmailNotificationPublisher(
    IAmazonSQS sqsClient,
    IOptions<SqsSettings> sqsSettings,
    ILogger<EmailNotificationPublisher> logger,
    ICorrelationIdAccessor correlation) : IEmailNotificationPublisher
{
    public async Task PublishAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[users-service] Publishing email welcome to SQS: To={To}, Subject={Subject}",
            to, subject);

        var messageBody = JsonSerializer.Serialize(new
        {
            To = to,
            Subject = subject,
            Body = body,
            CorrelationId = correlation.CorrelationId.ToString()
        });

        await sqsClient.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = sqsSettings.Value.EmailQueueUrl,
            MessageBody = messageBody
        }, cancellationToken);
    }
}
