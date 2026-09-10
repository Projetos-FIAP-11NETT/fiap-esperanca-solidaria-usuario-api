using FiapEsperancaSolidaria.Usuario.Auth.Adapter;
using FiapEsperancaSolidaria.Usuario.Shared.Option;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiapEsperancaSolidaria.Usuario.Auth.Configurations.Firebase;

public static class FirebaseConfig
{
    public static void AddFirebase(this IServiceCollection services, IConfiguration configuration)
    {
        var firebaseOptions = configuration.GetSection("Firebase").Get<FirebaseOptions>();

        if (firebaseOptions == null || string.IsNullOrWhiteSpace(firebaseOptions.CredencialJson))
            throw new InvalidOperationException("Firebase credential json não configurado.");

        var credential = CredentialFactory.FromJson<ServiceAccountCredential>(firebaseOptions.CredencialJson).ToGoogleCredential();

        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = credential
            });
        }

        services.AddScoped<IFirebaseService, FirebaseService>();
    }
}