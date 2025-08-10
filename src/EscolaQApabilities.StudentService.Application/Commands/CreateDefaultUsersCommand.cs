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

            // Criar usuário Admin
            if (!await _userRepository.ExistsByEmailAsync("admin@qapabilities.com"))
            {
                var adminPasswordHash = _passwordHasher.HashPassword("admin123");
                var adminUser = new User("admin@qapabilities.com", adminPasswordHash, "Admin", "Administrador");
                await _userRepository.AddAsync(adminUser);
                usersCreated++;
                _logger.LogInformation("Created default admin user");
            }

            // Criar usuário Teacher
            if (!await _userRepository.ExistsByEmailAsync("teacher@qapabilities.com"))
            {
                var teacherPasswordHash = _passwordHasher.HashPassword("teacher123");
                var teacherUser = new User("teacher@qapabilities.com", teacherPasswordHash, "Teacher", "Professor");
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
