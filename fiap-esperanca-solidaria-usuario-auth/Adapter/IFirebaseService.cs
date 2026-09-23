using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;

namespace FiapEsperancaSolidaria.Usuario.Auth.Adapter;

public interface IFirebaseService
{
    Task<string> CreateUserAsync(string email, string password, string name);

    Task UpdateUserAsync(string firebaseUserId, string email, string name);

    Task SetUserRoleAsync(string firebaseUserId, IEnumerable<string> roles, Guid? userId = null);
    Task SetUserIdAsync(string firebaseUserId, Guid userId);
    Task<LoginResponse> LoginUserAsync(string email, string password);
}
