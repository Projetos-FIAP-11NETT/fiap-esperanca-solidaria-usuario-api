using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using FiapEsperancaSolidaria.Usuario.Shared.Option;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace FiapEsperancaSolidaria.Usuario.Auth.Adapter;

public class FirebaseService(
    HttpClient httpClient,
    IOptions<FirebaseOptions> firebaseOptions
) : IFirebaseService
{
    private readonly FirebaseOptions _firebaseOptions = firebaseOptions.Value;

    public async Task<string> CreateUserAsync(string email, string password, string name)
    {
        var args = new UserRecordArgs
        {
            Email = email,
            Password = password,
            DisplayName = name,
            EmailVerified = false,
            Disabled = false
        };

        var userRecord = await FirebaseAuth
            .DefaultInstance
            .CreateUserAsync(args);

        return userRecord.Uid;
    }

    public async Task UpdateUserAsync(string firebaseUserId, string email, string name)
    {
        var args = new UserRecordArgs
        {
            Uid = firebaseUserId,
            Email = email,
            DisplayName = name
        };

        await FirebaseAuth
            .DefaultInstance
            .UpdateUserAsync(args);
    }

    public async Task SetUserRoleAsync(string firebaseUserId, IEnumerable<string> roles, Guid? userId)
    {
        var claims = new Dictionary<string, object>
        {
            { "roles", roles.ToArray() }
        };

        if (userId != null)
        {
            claims.Add("system_user_id", userId);
        }
        await FirebaseAuth.DefaultInstance
            .SetCustomUserClaimsAsync(firebaseUserId, claims);
    }

    public async Task SetUserIdAsync(string firebaseUserId, Guid userId)
    {
        var systemUserId = new Dictionary<string, object>
        {
            { "system_user_id", userId }
        };
        await FirebaseAuth.DefaultInstance
            .SetCustomUserClaimsAsync(firebaseUserId, systemUserId);
    }

    public async Task<LoginResponse> LoginUserAsync(string email, string password)
    {

        var userRecord = await FirebaseAuth
            .DefaultInstance
            .GetUserByEmailAsync(email);

        if (userRecord.Email == null)
            throw new UnauthorizedAccessException("Usuário não encontrado");

        var payload = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await httpClient.PostAsync(
            $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseOptions.ApiKey}",
            content
        );

        var errorJson = await response.Content.ReadAsStringAsync();


        if (!response.IsSuccessStatusCode)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos");

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var idToken = doc.RootElement.GetProperty("idToken").GetString();
        var expiresIn = int.Parse(doc.RootElement.GetProperty("expiresIn").GetString());
        var emailUser = doc.RootElement.GetProperty("email").GetString();
        var refreshToken = doc.RootElement.GetProperty("refreshToken").GetString();


        return new LoginResponse
        {
            IdToken = idToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn,
            Email = emailUser
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        var payload = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        };

        var response = await httpClient.PostAsync(
            $"https://securetoken.googleapis.com/v1/token?key={_firebaseOptions.ApiKey}",
            new FormUrlEncodedContent(payload)
        );

        if (!response.IsSuccessStatusCode)
            throw new UnauthorizedAccessException("Refresh token inválido");

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var idToken = doc.RootElement.GetProperty("id_token").GetString();
        var newRefreshToken = doc.RootElement.GetProperty("refresh_token").GetString();
        var expiresIn = int.Parse(doc.RootElement.GetProperty("expires_in").GetString());

        return new LoginResponse
        {
            IdToken = idToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = expiresIn
        };
    }
}
