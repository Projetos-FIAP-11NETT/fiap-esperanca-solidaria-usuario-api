using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories.Generic;
using FiapEsperancaSolidaria.Usuario.Domain.Entities;

namespace FiapEsperancaSolidaria.Usuario.Domain.Contracts.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role> FindDoadorRoleAsync();

    Task<Role> FindGestorRoleAsync();
}