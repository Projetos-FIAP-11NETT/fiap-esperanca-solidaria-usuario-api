using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories;
using FiapEsperancaSolidaria.Usuario.Domain.Entities;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Data;
using FiapEsperancaSolidaria.Usuario.Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace FiapEsperancaSolidaria.Usuario.Infrastructure.Repositories;

public class UserRepository
    (
        AppDbContext dataContext
    )
    : Repository<User>(dataContext), IUserRepository
{
    private readonly AppDbContext _dataContext = dataContext;

    public async Task<bool> ExistsEmailAsync(string email)
    {
        var exists = await _dataContext.Users
            .AnyAsync(u => u.Email == email);

        return exists;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var user = await _dataContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(s => s.Email == email);

        return user;
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id)
    {
        var user = await _dataContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(s => s.Id == id);

        return user;
    }
}
