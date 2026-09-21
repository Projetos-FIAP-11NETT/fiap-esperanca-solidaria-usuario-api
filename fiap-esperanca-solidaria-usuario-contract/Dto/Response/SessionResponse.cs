namespace FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
public class SessionResponse
{
    public Guid SessionId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}