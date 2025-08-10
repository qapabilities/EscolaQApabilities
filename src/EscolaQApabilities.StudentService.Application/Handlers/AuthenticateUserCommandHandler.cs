using MediatR;
using Microsoft.Extensions.Logging;
using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Application.Services;
using EscolaQApabilities.StudentService.Application.Commands;
using EscolaQApabilities.StudentService.Application.DTOs;

namespace EscolaQApabilities.StudentService.Application.Handlers;

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, AuthenticationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthenticateUserCommandHandler> _logger;

    public AuthenticateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        ILogger<AuthenticateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthenticationResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Normalizar email
            var email = request.Email.ToLowerInvariant().Trim();

            // Buscar usuário
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", email);
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Credenciais inválidas"
                };
            }

            // Verificar se usuário está ativo
            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {Email}", email);
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Conta desativada"
                };
            }

            // Verificar se conta está bloqueada
            if (user.IsLocked())
            {
                _logger.LogWarning("Login attempt for locked account: {Email}", email);
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Conta bloqueada até {user.LockedUntil:HH:mm}",
                    IsLocked = true,
                    LockedUntil = user.LockedUntil
                };
            }

            // Verificar senha
            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                user.IncrementLoginAttempts();
                await _userRepository.UpdateAsync(user);

                _logger.LogWarning("Failed login attempt for user: {Email}, Attempts: {Attempts}", 
                    email, user.LoginAttempts);

                var errorMessage = user.IsLocked() 
                    ? $"Conta bloqueada por 15 minutos devido a múltiplas tentativas falhadas"
                    : "Credenciais inválidas";

                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = errorMessage,
                    IsLocked = user.IsLocked(),
                    LockedUntil = user.LockedUntil
                };
            }

            // Login bem-sucedido
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            // Gerar token JWT
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role);

            _logger.LogInformation("Successful login for user: {Email}", email);

            return new AuthenticationResult
            {
                IsSuccess = true,
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for email: {Email}", request.Email);
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "Erro interno durante autenticação"
            };
        }
    }
}
