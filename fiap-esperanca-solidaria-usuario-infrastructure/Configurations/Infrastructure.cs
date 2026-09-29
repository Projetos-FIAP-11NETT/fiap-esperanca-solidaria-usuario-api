using Amazon.S3;
using FiapEsperancaSolidaria.Usuario.Application.Sessions;
using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories;
using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Storage;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Cache;
using FiapEsperancaSolidaria.Usuario.Infrastructure.CurrentUser;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Data;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Repositories;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Storage;
using FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FiapEsperancaSolidaria.Usuario.Infrastructure.Configurations;

public static class Infrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "FiapEsperancaSolidaria.Usuario:";
        });

        services.AddScoped<ISessionCacheService, RedisSessionCacheService>();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // Mesmo padrão da campanha-api: LocalStack em dev (ServiceUrl setado), AWS real em
        // produção (sem credenciais explícitas, usa a IAM role do node via IMDS).
        services.Configure<S3Settings>(configuration.GetSection("S3Settings"));
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;

            if (!string.IsNullOrWhiteSpace(settings.ServiceUrl))
            {
                return new AmazonS3Client(settings.AccessKey, settings.SecretKey, new AmazonS3Config
                {
                    ServiceURL = settings.ServiceUrl,
                    ForcePathStyle = true,
                    AuthenticationRegion = settings.Region,
                });
            }

            return new AmazonS3Client(new AmazonS3Config { AuthenticationRegion = settings.Region });
        });
        services.AddSingleton<IImageStorageService, S3ImageStorageService>();

        return services;
    }
}

