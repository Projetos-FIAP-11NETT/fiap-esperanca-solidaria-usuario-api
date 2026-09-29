using FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiapEsperancaSolidaria.Usuario.Infrastructure.Correlation;

public sealed class CorrelationIdAccessor : ICorrelationIdAccessor
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CorrelationId =>
           _httpContextAccessor.HttpContext?.Items[HeaderName] as string ?? Guid.NewGuid().ToString();

}