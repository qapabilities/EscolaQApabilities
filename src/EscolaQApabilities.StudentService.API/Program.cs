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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Service API V1");
    });
}

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

app.MapControllers();

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