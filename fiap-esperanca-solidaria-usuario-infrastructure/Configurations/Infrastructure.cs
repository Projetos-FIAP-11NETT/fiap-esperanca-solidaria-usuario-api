using FiapEsperancaSolidaria.Usuario.Application.Sessions;
using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Cache;
using FiapEsperancaSolidaria.Usuario.Infrastructure.CurrentUser;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Data;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Repositories;
using FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        return services;
    }
}

