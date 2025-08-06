using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Infrastructure.Data;
using EscolaQApabilities.StudentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EscolaQApabilities.StudentService.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar Entity Framework
        services.AddDbContext<StudentDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("EscolaQApabilities.StudentService.Infrastructure")));

        // Registrar repositórios
        services.AddScoped<IStudentRepository, StudentRepository>();

        return services;
    }
} 