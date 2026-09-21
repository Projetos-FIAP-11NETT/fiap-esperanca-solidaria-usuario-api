using System;
using System.Collections.Generic;
using System.Text;

namespace FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
public class LoginResponse
{
    public Guid SessionId { get; set; }
    public string IdToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public string Email { get; set; }
}
