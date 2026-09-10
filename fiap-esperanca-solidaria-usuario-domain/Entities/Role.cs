namespace FiapEsperancaSolidaria.Usuario.Domain.Entities;

public class Role
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    private readonly List<User> _users = [];
    public IReadOnlyCollection<User> Users => _users;

    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }
}