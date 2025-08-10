using System.Security.Claims;

namespace EscolaQApabilities.StudentService.Application.Services;

public interface IJwtService
{
    string GenerateToken(string userId, string email, string role);
    ClaimsPrincipal? ValidateToken(string token);
}

