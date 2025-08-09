using EscolaQApabilities.StudentService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EscolaQApabilities.StudentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IJwtService jwtService, ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// Endpoint para login (simulado para demonstração)
    /// </summary>
    /// <param name="loginRequest">Dados de login</param>
    /// <returns>Token JWT</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest loginRequest)
    {
        // Simulação de autenticação - em produção, validar contra banco de dados
        if (IsValidUser(loginRequest.Email, loginRequest.Password))
        {
            var role = GetUserRole(loginRequest.Email);
            var userId = Guid.NewGuid().ToString(); // Em produção, pegar do banco

            var token = _jwtService.GenerateToken(userId, loginRequest.Email, role);

            _logger.LogInformation("Login successful for user: {Email}", loginRequest.Email);

            return Ok(new LoginResponse
            {
                Token = token,
                ExpiresIn = 3600, // 1 hora
                User = new UserInfo
                {
                    Id = userId,
                    Email = loginRequest.Email,
                    Role = role
                }
            });
        }

        _logger.LogWarning("Failed login attempt for user: {Email}", loginRequest.Email);
        return Unauthorized(new { Message = "Credenciais inválidas" });
    }

    /// <summary>
    /// Endpoint para validar token
    /// </summary>
    /// <returns>Informações do usuário autenticado</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UserInfo> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        return Ok(new UserInfo
        {
            Id = userId,
            Email = email,
            Role = role
        });
    }

    private bool IsValidUser(string email, string password)
    {
        // Simulação - em produção, validar contra banco de dados
        return email == "admin@qapabilities.com" && password == "admin123" ||
               email == "teacher@qapabilities.com" && password == "teacher123";
    }

    private string GetUserRole(string email)
    {
        return email == "admin@qapabilities.com" ? "Admin" : "Teacher";
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
