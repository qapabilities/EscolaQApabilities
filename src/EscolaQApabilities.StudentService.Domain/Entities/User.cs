using EscolaQApabilities.StudentService.Domain.Exceptions;

namespace EscolaQApabilities.StudentService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int LoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    private User() { } // Para EF Core

    public User(string email, string passwordHash, string role, string name)
    {
        Id = Guid.NewGuid();
        SetEmail(email);
        PasswordHash = passwordHash;
        SetRole(role);
        Name = name;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        LoginAttempts = 0;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new UserDomainException("Email é obrigatório");

        if (!IsValidEmail(email))
            throw new UserDomainException("Email inválido. Formato correto: email@dominio.com");

        Email = email.ToLowerInvariant();
    }

    public void SetRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new UserDomainException("Role é obrigatória");

        var validRoles = new[] { "Admin", "Teacher" };
        if (!validRoles.Contains(role))
            throw new UserDomainException($"Role inválida. Roles válidas: {string.Join(", ", validRoles)}");

        Role = role;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        LoginAttempts = 0; // Reset login attempts on successful login
    }

    public void IncrementLoginAttempts()
    {
        LoginAttempts++;
        
        // Lock account after 5 failed attempts for 15 minutes
        if (LoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        }
    }

    public bool IsLocked()
    {
        return LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
    }

    public void Unlock()
    {
        LoginAttempts = 0;
        LockedUntil = null;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public class UserDomainException : Exception
{
    public UserDomainException(string message) : base(message) { }
}
