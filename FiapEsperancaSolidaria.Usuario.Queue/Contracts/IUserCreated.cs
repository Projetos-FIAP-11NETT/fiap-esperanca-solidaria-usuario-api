namespace FiapEsperancaSolidaria.Usuario.Queue.Contracts;

public interface IUserCreated
{
    public Guid Id { get; }
    public string Email { get; }
    public string Name { get; }
}
