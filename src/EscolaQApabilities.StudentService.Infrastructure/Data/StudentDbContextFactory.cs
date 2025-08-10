using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using EscolaQApabilities.StudentService.Infrastructure.Configuration;
using System.IO;

namespace EscolaQApabilities.StudentService.Infrastructure.Data;

public class StudentDbContextFactory : IDesignTimeDbContextFactory<StudentDbContext>
{
    public StudentDbContext CreateDbContext(string[] args)
    {
        // Carrega configuração de appsettings.json + appsettings.{Environment}.json
        var environmentName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Priorizar variáveis de ambiente para maior segurança
        var connectionString = System.Environment.GetEnvironmentVariable("STUDENT_DB_CONNECTION_STRING")
            ?? configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (environmentName.Equals("Production", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Database connection string not found in environment variable 'STUDENT_DB_CONNECTION_STRING'. " +
                    "In production, always use environment variables for database credentials.");
            }
            else
            {
                throw new InvalidOperationException(
                    "Database connection string not found in configuration or environment variable 'STUDENT_DB_CONNECTION_STRING'.");
            }
        }

        var optionsBuilder = new DbContextOptionsBuilder<StudentDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly("EscolaQApabilities.StudentService.Infrastructure");
        });

        return new StudentDbContext(optionsBuilder.Options);
    }
} 