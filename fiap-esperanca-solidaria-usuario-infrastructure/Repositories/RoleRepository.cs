using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories;
using FiapEsperancaSolidaria.Usuario.Domain.Entities;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Data;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;


namespace FiapEsperancaSolidaria.Usuario.Infrastructure.Repositories;

public class RoleRepository(AppDbContext dataContext) : Repository<Role>(dataContext), IRoleRepository
{
    private readonly AppDbContext _dataContext = dataContext;

    public async Task<Role> FindUserRoleAsync()
    {
        var role = await _dataContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "Doador");

        return role;
    }

    public async Task<Role> FindAdminRoleAsync()
    {
        var role = await _dataContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "GestorONG");

        return role;
    }
}
