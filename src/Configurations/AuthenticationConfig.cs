using FiapEsperancaSolidaria.Usuario.Shared.Option;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FiapEsperancaSolidaria.Usuario.Api.Configurations;

public static class AuthenticationConfig
{
    public static void AddAuthenticationConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var firebaseOptions = configuration
            .GetSection("Firebase")
            .Get<FirebaseOptions>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.Authority = $"https://securetoken.google.com/{firebaseOptions.ProjectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{firebaseOptions.ProjectId}",
                    ValidateAudience = true,
                    ValidAudience = firebaseOptions.ProjectId,
                    ValidateLifetime = true
                };
        });
        services.AddAuthorization();
    }
}
