using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Infrastructure.Data;
using EscolaQApabilities.StudentService.Infrastructure.Repositories;
using EscolaQApabilities.StudentService.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EscolaQApabilities.StudentService.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar Entity Framework
        var connectionString = Environment.GetEnvironmentVariable("STUDENT_DB_CONNECTION_STRING")
            ?? configuration.GetConnectionString("DefaultConnection");
        var useSqlite = Environment.GetEnvironmentVariable("USE_SQLITE") == "true";
            
        services.AddDbContext<StudentDbContext>(options =>
        {
            if (useSqlite)
            {
                options.UseSqlite(connectionString ?? "Data Source=:memory:",
                    b => b.MigrationsAssembly("EscolaQApabilities.StudentService.Infrastructure"));
            }
            else
            {
                options.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly("EscolaQApabilities.StudentService.Infrastructure"));
            }
        });

        // Registrar repositórios
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
