using EscolaQApabilities.StudentService.API.Services;
using EscolaQApabilities.StudentService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EscolaQApabilities.StudentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Endpoint para login seguro com validação contra banco de dados
    /// </summary>
    /// <param name="loginRequest">Dados de login</param>
    /// <returns>Token JWT</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status423Locked)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        var command = new AuthenticateUserCommand
        {
            Email = loginRequest.Email,
            Password = loginRequest.Password
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(new LoginResponse
            {
                Token = result.Token!,
                ExpiresIn = 3600, // 1 hora
                User = new UserInfo
                {
                    Id = result.User!.Id.ToString(),
                    Email = result.User.Email,
                    Role = result.User.Role
                }
            });
        }

        if (result.IsLocked)
        {
            return StatusCode(StatusCodes.Status423Locked, new { 
                Message = result.ErrorMessage,
                LockedUntil = result.LockedUntil
            });
        }

        return Unauthorized(new { Message = result.ErrorMessage });
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
