using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;

namespace FiapEsperancaSolidaria.Usuario.Auth;

public interface IAuthService
{
    Task<string> CreateUserAsync(string email, string password, string name, IEnumerable<string> roles, Guid userId);

    Task UpdateUserAsync(string firebaseUserId, string email, string name);

    Task SetUserRoleAsync(string firebaseUserId, IEnumerable<string> roles, Guid idUser);

    Task<LoginResponse> LoginUserAsync(string email, string password);
}
