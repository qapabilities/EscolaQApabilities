using EscolaQApabilities.StudentService.Application.Commands;
using EscolaQApabilities.StudentService.Application.Queries;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using EscolaQApabilities.StudentService.Infrastructure.DependencyInjection;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Escola QApabilities - Student Service API", Version = "v1" });
});

// Configurar MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateStudentCommand).Assembly));

// Configurar Infraestrutura
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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

app.UseCors("AllowAll");

app.UseAuthorization();

// Configurar tratamento de exceções
app.UseExceptionHandler("/error");

app.MapControllers();

app.Run(); 