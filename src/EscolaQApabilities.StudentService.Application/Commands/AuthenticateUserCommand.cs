using MediatR;
using EscolaQApabilities.StudentService.Application.DTOs;

namespace EscolaQApabilities.StudentService.Application.Commands;

public class AuthenticateUserCommand : IRequest<AuthenticationResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string? Token { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedUntil { get; set; }
}

