using MediatR;
using Microsoft.Extensions.Logging;
using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Application.Services;

namespace EscolaQApabilities.StudentService.Application.Commands;

public class CreateDefaultUsersCommand : IRequest<bool>
{
}

public class CreateDefaultUsersCommandHandler : IRequestHandler<CreateDefaultUsersCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateDefaultUsersCommandHandler> _logger;

    public CreateDefaultUsersCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<CreateDefaultUsersCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<bool> Handle(CreateDefaultUsersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var usersCreated = 0;

            // Obter credenciais de variáveis de ambiente (NUNCA hardcode em produção!)
            var adminEmail = Environment.GetEnvironmentVariable("DEFAULT_ADMIN_EMAIL") ?? "admin@qapabilities.com";
            var adminPassword = Environment.GetEnvironmentVariable("DEFAULT_ADMIN_PASSWORD") ?? "admin123";
            var teacherEmail = Environment.GetEnvironmentVariable("DEFAULT_TEACHER_EMAIL") ?? "teacher@qapabilities.com";
            var teacherPassword = Environment.GetEnvironmentVariable("DEFAULT_TEACHER_PASSWORD") ?? "teacher123";

            // AVISO: Em produção, sempre defina as variáveis de ambiente!
            if (adminPassword == "admin123" || teacherPassword == "teacher123")
            {
                _logger.LogWarning("⚠️ USANDO SENHAS PADRÃO! Em produção, configure as variáveis: DEFAULT_ADMIN_PASSWORD, DEFAULT_TEACHER_PASSWORD");
            }

            // Criar usuário Admin
            if (!await _userRepository.ExistsByEmailAsync(adminEmail))
            {
                var adminPasswordHash = _passwordHasher.HashPassword(adminPassword);
                var adminUser = new User(adminEmail, adminPasswordHash, "Admin", "Administrador");
                await _userRepository.AddAsync(adminUser);
                usersCreated++;
                _logger.LogInformation("Created default admin user");
            }

            // Criar usuário Teacher
            if (!await _userRepository.ExistsByEmailAsync(teacherEmail))
            {
                var teacherPasswordHash = _passwordHasher.HashPassword(teacherPassword);
                var teacherUser = new User(teacherEmail, teacherPasswordHash, "Teacher", "Professor");
                await _userRepository.AddAsync(teacherUser);
                usersCreated++;
                _logger.LogInformation("Created default teacher user");
            }

            if (usersCreated > 0)
            {
                _logger.LogInformation("Created {Count} default users", usersCreated);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default users");
            return false;
        }
    }
}
