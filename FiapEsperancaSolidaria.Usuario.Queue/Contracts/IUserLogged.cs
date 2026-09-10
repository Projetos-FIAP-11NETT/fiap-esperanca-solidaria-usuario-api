namespace FiapEsperancaSolidaria.Usuario.Queue.Contracts;

public interface IUserLogged
{
    public string IdToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public string Email { get; set; }
}
