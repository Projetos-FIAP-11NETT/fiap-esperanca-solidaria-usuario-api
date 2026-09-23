using FiapEsperancaSolidaria.Usuario.Auth.Adapter;
using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using FiapEsperancaSolidaria.Usuario.Domain.Exceptions;
using FirebaseAdmin;
using FirebaseAdmin.Auth;

namespace FiapEsperancaSolidaria.Usuario.Auth;

public sealed class AuthService
    (
        IFirebaseService client
    )
    : IAuthService
{
    public async Task<string> CreateUserAsync(string email, string password, string name, IEnumerable<string> roles, Guid userId)
    {
        var firebaseUserId = await client.CreateUserAsync(email, password, name);
        if (string.IsNullOrEmpty(firebaseUserId))
            return string.Empty;

        await client.SetUserRoleAsync(firebaseUserId, roles, userId);

        return firebaseUserId;
    }

    public async Task SetUserRoleAsync(string firebaseUserId, IEnumerable<string> roles, Guid idUser)
    {
        await client.SetUserRoleAsync(firebaseUserId, roles, idUser);
    }

    public async Task UpdateUserAsync(string firebaseUserId, string email, string name)
    {
        try
        {
            await client.UpdateUserAsync(firebaseUserId, email, name);
        }
        catch (FirebaseAuthException ex)
        {
            throw FirebaseExceptionMapper.Map(ex);
        }
        catch (FirebaseException ex)
        {
            throw new ExternalException("Firebase", ex.Message);
        }
    }

    public async Task<LoginResponse> LoginUserAsync(string email, string password)
    {
        try
        {
            return await client.LoginUserAsync(email, password);
        }
        catch (FirebaseAuthException ex)
        {
            throw FirebaseExceptionMapper.Map(ex);
        }
        catch (FirebaseException ex)
        {
            throw new ExternalException("Firebase", ex.Message);
        }
    }
}
