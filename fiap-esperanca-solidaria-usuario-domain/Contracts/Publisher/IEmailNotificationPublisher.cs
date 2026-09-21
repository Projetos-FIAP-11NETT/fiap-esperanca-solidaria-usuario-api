namespace FiapEsperancaSolidaria.Usuario.Domain.Contracts.Publisher;

public interface IEmailNotificationPublisher
{
    Task PublishAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
