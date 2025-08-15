using EscolaQApabilities.StudentService.Application.Commands;
using EscolaQApabilities.StudentService.Application.Queries;
using EscolaQApabilities.StudentService.Application.Services;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using EscolaQApabilities.StudentService.Infrastructure.DependencyInjection;
using EscolaQApabilities.StudentService.API.Services;
using EscolaQApabilities.StudentService.API.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add health checks
builder.Services.AddHealthChecks();

// Configurar rotas em lowercase
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Escola QApabilities - Student Service API", Version = "v1" });
    
    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configurar MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateStudentCommand).Assembly));

// Configurar Infraestrutura
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar JWT Configuration
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(JwtConfiguration.SectionName));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtConfiguration>>().Value);

// Configurar JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Configurar serviços de autenticação
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();



// Configurar Autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtConfig = builder.Configuration.GetSection(JwtConfiguration.SectionName).Get<JwtConfiguration>();
        if (jwtConfig == null)
        {
            throw new InvalidOperationException("JWT configuration not found");
        }
        jwtConfig.Validate();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
            ValidateIssuer = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtConfig.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configurar Autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TeacherOrAdmin", policy => policy.RequireRole("Admin", "Teacher"));
});

// Configurar Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = Convert.ToInt32(builder.Configuration["Security:RateLimit:PermitLimit"]);
        limiterOptions.Window = TimeSpan.FromMinutes(Convert.ToInt32(builder.Configuration["Security:RateLimit:WindowMinutes"]));

        limiterOptions.QueueLimit = 2;
    });
});

// Configurar CORS Restritivo
builder.Services.AddCors(options =>
{
    options.AddPolicy("RestrictedPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Em desenvolvimento, permitir todas as origens
            policy.AllowAnyOrigin()
                  .WithMethods("GET", "POST", "PUT", "DELETE")
                  .WithHeaders("Authorization", "Content-Type");
        }
        else
        {
            // Em produção, usar origens restritas
            var allowedOrigins = builder.Configuration.GetSection("Security:AllowedOrigins").Get<string[]>() ?? new[] { "https://escola-qapabilities.com" };
            
            policy.WithOrigins(allowedOrigins)
                  .WithMethods("GET", "POST", "PUT", "DELETE")
                  .WithHeaders("Authorization", "Content-Type")
                  .AllowCredentials();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Swagger habilitado em todos os ambientes (incluindo produção)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Service API V1");
    c.RoutePrefix = "swagger"; // Swagger disponível em /swagger
});

// Configurar pipeline de segurança
app.UseCors("RestrictedPolicy");

// Adicionar headers de segurança
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Configurar Rate Limiting
app.UseRateLimiter();

// Configurar tratamento de exceções
app.UseExceptionHandler("/error");

// Configure health checks endpoint
app.MapHealthChecks("/health");

app.MapControllers();

// Executar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EscolaQApabilities.StudentService.Infrastructure.Data.StudentDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Applying database migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");
        
        // Garantir que as tabelas existem criando-as via SQL direto
        logger.LogInformation("Creating tables manually...");
        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""Users"" (
                ""Id"" TEXT NOT NULL,
                ""Email"" TEXT NOT NULL,
                ""PasswordHash"" TEXT NOT NULL,
                ""Role"" TEXT NOT NULL,
                ""Name"" TEXT NOT NULL,
                ""IsActive"" INTEGER NOT NULL,
                ""CreatedAt"" TEXT NOT NULL,
                ""LastLoginAt"" TEXT NULL,
                ""LoginAttempts"" INTEGER NOT NULL,
                ""LockedUntil"" TEXT NULL,
                CONSTRAINT ""PK_Users"" PRIMARY KEY (""Id"")
            );
            
            CREATE TABLE IF NOT EXISTS ""Students"" (
                ""Id"" TEXT NOT NULL,
                ""Name"" TEXT NOT NULL,
                ""Email"" TEXT NOT NULL,
                ""BirthDate"" TEXT NOT NULL,
                ""Phone"" TEXT NOT NULL,
                ""Address"" TEXT NOT NULL,
                ""City"" TEXT NOT NULL,
                ""State"" TEXT NOT NULL,
                ""ZipCode"" TEXT NOT NULL,
                ""Status"" INTEGER NOT NULL,
                ""EnrollmentDate"" TEXT NOT NULL,
                ""ParentName"" TEXT NULL,
                ""ParentPhone"" TEXT NULL,
                ""ParentEmail"" TEXT NULL,
                ""EmergencyContact"" TEXT NULL,
                ""EmergencyPhone"" TEXT NULL,
                ""MedicalInformation"" TEXT NULL,
                ""Notes"" TEXT NULL,
                ""CreatedAt"" TEXT NOT NULL,
                ""UpdatedAt"" TEXT NOT NULL,
                CONSTRAINT ""PK_Students"" PRIMARY KEY (""Id"")
            );
            
            CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Users_Email"" ON ""Users"" (""Email"");
            CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Students_Email"" ON ""Students"" (""Email"");
        ");
        logger.LogInformation("Tables created successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error applying database migrations");
    }
}

// Inicializar usuários padrão
using (var scope = app.Services.CreateScope())
{
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    try
    {
        await mediator.Send(new EscolaQApabilities.StudentService.Application.Commands.CreateDefaultUsersCommand());
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error initializing default users");
    }
}

app.Run(); 