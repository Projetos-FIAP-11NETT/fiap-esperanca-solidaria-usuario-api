namespace FiapEsperancaSolidaria.Usuario.Shared.Abstractions;

public interface ICurrentUserAccessor
{
    Guid? UserId { get; }
}
