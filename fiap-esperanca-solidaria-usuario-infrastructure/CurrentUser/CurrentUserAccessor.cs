using FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
using Microsoft.AspNetCore.Http;

namespace FiapEsperancaSolidaria.Usuario.Infrastructure.CurrentUser;

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public Guid? UserId
    {
        get
        {
            var claimValue = httpContextAccessor.HttpContext?.User.FindFirst("system_user_id")?.Value;

            return Guid.TryParse(claimValue, out var userId)
                ? userId
                : null;
        }
    }
}
