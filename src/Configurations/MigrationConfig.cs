namespace FiapEsperancaSolidaria.Usuario.Api.Configurations;

using FiapEsperancaSolidaria.Usuario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public static class MigrationConfig
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        var scope = app.ApplicationServices.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dataContext.Database.Migrate();
    }
}