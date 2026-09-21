using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories.Generic;
using FiapEsperancaSolidaria.Usuario.Domain.Entities;

namespace FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    Task<bool> ExistsEmailAsync(string email);
}