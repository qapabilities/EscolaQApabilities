using EscolaQApabilities.StudentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EscolaQApabilities.StudentService.Infrastructure.Data;

public class StudentDbContext : DbContext
{
    public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Student
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever(); // Usar GUID gerado pela aplicação
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(15);
                
            entity.Property(e => e.BirthDate)
                .IsRequired();
                
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.State)
                .IsRequired()
                .HasMaxLength(2);
                
            entity.Property(e => e.ZipCode)
                .IsRequired()
                .HasMaxLength(8);
                
            entity.Property(e => e.Status)
                .IsRequired();
                
            entity.Property(e => e.EnrollmentDate)
                .IsRequired();
                
            entity.Property(e => e.ParentName)
                .HasMaxLength(100);
                
            entity.Property(e => e.ParentPhone)
                .HasMaxLength(15);
                
            entity.Property(e => e.ParentEmail)
                .HasMaxLength(100);
                
            entity.Property(e => e.EmergencyContact)
                .HasMaxLength(100);
                
            entity.Property(e => e.EmergencyPhone)
                .HasMaxLength(15);
                
            entity.Property(e => e.MedicalInformation)
                .HasMaxLength(500);
                
            entity.Property(e => e.Notes)
                .HasMaxLength(1000);
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Índices
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.City);
            entity.HasIndex(e => e.EnrollmentDate);
        });
    }
} 