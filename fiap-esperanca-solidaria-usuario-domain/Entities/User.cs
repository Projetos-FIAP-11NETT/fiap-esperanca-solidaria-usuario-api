using FiapEsperancaSolidaria.Usuario.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace FiapEsperancaSolidaria.Usuario.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string FirebaseUserId { get; private set; }

    private readonly List<Role> _roles = [];
    public IReadOnlyCollection<Role> Roles => _roles;

    [NotMapped]
    public string Password { get; private set; }

    private User() { }


    public User(string name, string email, string password, Role role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = password;

        AddRole(role);
        Validate();
    }

    public void SetFirebaseUserId(string firebaseUserId)
    {
        if (string.IsNullOrWhiteSpace(firebaseUserId))
            throw new DomainException("FirebaseUserId inválido. Não pode ser vazio.");

        FirebaseUserId = firebaseUserId;
    }

    public void AddRole(Role role)
    {
        if (role is null)
            throw new DomainException($"Perfil inválido. Não pode ser vazio.");

        if (_roles.Contains(role))
            return;

        _roles.Add(role);
    }

    public void MakeAdmin(Role role)
    {
        _roles.Clear();
        AddRole(role);
    }

    #region Validate for create user

    private void Validate()
    {
        ValidateName();
        ValidateEmail();
        ValidatePassword();
    }

    private void ValidateName()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("Nome inválido. Não pode ser vazio");

        if (Name.Length > 50)
            throw new DomainException("Nome inválido. Deve ter no máximo 50 caracteres.");
    }

    private void ValidateEmail()
    {
        if (string.IsNullOrWhiteSpace(Email))
            throw new DomainException("E-mail inválido. Não pode ser vazio.");

        if (Email.Length > 255)
            throw new DomainException("E-mail inválido. Deve ter no máximo 255 caracteres.");

        var isValid = Regex.IsMatch(
            Email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase,
            TimeSpan.FromMilliseconds(250)
        );

        if (!isValid)
            throw new DomainException("E-mail inválido. Formato incorreto.");
    }

    private void ValidatePassword()
    {
        if (string.IsNullOrWhiteSpace(Password))
            throw new DomainException("Senha inválida. Não pode ser vazia.");

        if (Password.Length < 8)
            throw new DomainException("Senha inválida. Deve ter no mínimo 8 caracteres.");

        bool hasLetter = false, hasDigit = false, hasSpecialChar = false;

        foreach (char c in Password)
        {
            if (char.IsLetter(c))
                hasLetter = true;

            else if (char.IsDigit(c))
                hasDigit = true;

            else if (!char.IsWhiteSpace(c) && !char.IsControl(c))
                hasSpecialChar = true;
        }

        if (!hasLetter)
            throw new DomainException("Senha inválida. Deve conter pelo menos uma letra.");

        if (!hasDigit)
            throw new DomainException("Senha inválida. Deve conter pelo menos um número.");

        if (!hasSpecialChar)
            throw new DomainException("Senha inválida. Deve conter pelo menos um caractere especial.");
    }

    #endregion validations
}