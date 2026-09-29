namespace FiapEsperancaSolidaria.Usuario.Application.Sessions;

public interface ISessionCacheService
{
    Task StoreAsync(SessionCacheEntry session, TimeSpan expiration, CancellationToken cancellationToken);
    Task<SessionCacheEntry?> GetAsync(Guid sessionId, CancellationToken cancellationToken);
    Task RemoveAsync(Guid sessionId, CancellationToken cancellationToken);
}