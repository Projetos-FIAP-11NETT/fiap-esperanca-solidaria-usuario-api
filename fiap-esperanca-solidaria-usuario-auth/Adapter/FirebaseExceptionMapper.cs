using FiapEsperancaSolidaria.Usuario.Domain.Exceptions;
using FirebaseAdmin.Auth;

namespace FiapEsperancaSolidaria.Usuario.Auth.Adapter;

internal static class FirebaseExceptionMapper
{
    public static Exception Map(FirebaseAuthException exception)
    {
        return exception.AuthErrorCode switch
        {
            AuthErrorCode.UserNotFound =>
                new NotFoundException("Usuário não encontrado"),

            AuthErrorCode.EmailNotFound =>
                new NotFoundException("Email não encontrado"),

            AuthErrorCode.EmailAlreadyExists =>
                new BusinessException("Email já cadastrado"),
            _ =>
                new ExternalException("Firebase", "Erro inesperado ao processar autenticação.")
        };
    }
}