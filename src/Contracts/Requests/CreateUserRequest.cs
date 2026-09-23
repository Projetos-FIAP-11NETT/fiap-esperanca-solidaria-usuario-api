namespace FiapEsperancaSolidaria.Usuario.Contracts.Requests;

public sealed record CreateUserRequest(
    string Name,
    string Email,
    string Password,
    string Cpf,
    string? Image);
