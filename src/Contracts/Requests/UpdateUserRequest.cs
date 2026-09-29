namespace FiapEsperancaSolidaria.Usuario.Contracts.Requests;

public sealed record UpdateUserRequest(
    string Name,
    string Email,
    string Cpf,
    string? Image);
