namespace FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
public interface ICorrelationIdAccessor
{
    Guid CorrelationId { get; }
}

