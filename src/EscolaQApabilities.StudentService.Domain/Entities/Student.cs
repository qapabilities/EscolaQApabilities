using EscolaQApabilities.StudentService.Domain.Enums;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using System.Linq;

namespace EscolaQApabilities.StudentService.Domain.Entities;

public class Student : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public StudentStatus Status { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public string? ParentName { get; private set; }
    public string? ParentPhone { get; private set; }
    public string? ParentEmail { get; private set; }
    public string? EmergencyContact { get; private set; }
    public string? EmergencyPhone { get; private set; }
    public string? MedicalInformation { get; private set; }
    public string? Notes { get; private set; }

    // Construtor privado para EF Core
    private Student() { }

    public Student(
        string name,
        string email,
        string phone,
        DateTime birthDate,
        string address,
        string city,
        string state,
        string zipCode,
        string? parentName = null,
        string? parentPhone = null,
        string? parentEmail = null,
        string? emergencyContact = null,
        string? emergencyPhone = null,
        string? medicalInformation = null,
        string? notes = null)
    {
        ValidateStudentData(name, email, phone, birthDate, address, city, state, zipCode);
        
        Name = name.Trim();
        Email = email.Trim().ToLower();
        Phone = phone.Trim();
        BirthDate = birthDate;
        Address = address.Trim();
        City = city.Trim();
        State = state.Trim();
        ZipCode = zipCode.Trim();
        Status = StudentStatus.Active;
        EnrollmentDate = DateTime.UtcNow;
        ParentName = parentName?.Trim();
        ParentPhone = parentPhone?.Trim();
        ParentEmail = parentEmail?.Trim()?.ToLower();
        EmergencyContact = emergencyContact?.Trim();
        EmergencyPhone = emergencyPhone?.Trim();
        MedicalInformation = medicalInformation?.Trim();
        Notes = notes?.Trim();
    }

    public void UpdatePersonalInfo(
        string name,
        string email,
        string phone,
        DateTime birthDate,
        string address,
        string city,
        string state,
        string zipCode)
    {
        ValidateStudentData(name, email, phone, birthDate, address, city, state, zipCode);
        
        Name = name.Trim();
        Email = email.Trim().ToLower();
        Phone = phone.Trim();
        BirthDate = birthDate;
        Address = address.Trim();
        City = city.Trim();
        State = state.Trim();
        ZipCode = zipCode.Trim();
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContactInfo(
        string? parentName,
        string? parentPhone,
        string? parentEmail,
        string? emergencyContact,
        string? emergencyPhone)
    {
        ParentName = parentName?.Trim();
        ParentPhone = parentPhone?.Trim();
        ParentEmail = parentEmail?.Trim()?.ToLower();
        EmergencyContact = emergencyContact?.Trim();
        EmergencyPhone = emergencyPhone?.Trim();
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMedicalInfo(string? medicalInformation)
    {
        MedicalInformation = medicalInformation?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == StudentStatus.Active)
            throw new StudentDomainException("Aluno já está ativo.");
            
        Status = StudentStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (Status == StudentStatus.Inactive)
            throw new StudentDomainException("Aluno já está inativo.");
            
        Status = StudentStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        if (Status == StudentStatus.Suspended)
            throw new StudentDomainException("Aluno já está suspenso.");
            
        Status = StudentStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - BirthDate.Year;
        
        if (BirthDate.Date > today.AddYears(-age))
            age--;
            
        return age;
    }

    public bool IsMinor()
    {
        return GetAge() < 18;
    }

    private static void ValidateStudentData(
        string name,
        string email,
        string phone,
        DateTime birthDate,
        string address,
        string city,
        string state,
        string zipCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new StudentDomainException("Nome é obrigatório.");
            
        if (name.Length < 2 || name.Length > 100)
            throw new StudentDomainException("Nome deve ter entre 2 e 100 caracteres.");
            
        if (string.IsNullOrWhiteSpace(email))
            throw new StudentDomainException("Email é obrigatório.");
            
        if (!IsValidEmail(email))
            throw new StudentDomainException("Email inválido. Formato correto: email@dominio.com");
            
        if (string.IsNullOrWhiteSpace(phone))
            throw new StudentDomainException("Telefone é obrigatório.");
            
        if (phone.Length < 10 || phone.Length > 15)
            throw new StudentDomainException("Telefone deve ter entre 10 e 15 caracteres.");
            
        if (birthDate >= DateTime.Today)
            throw new StudentDomainException("Data de nascimento deve ser anterior à data atual.");
            
        if (birthDate < DateTime.Today.AddYears(-100))
            throw new StudentDomainException("Data de nascimento inválida.");
            
        if (string.IsNullOrWhiteSpace(address))
            throw new StudentDomainException("Endereço é obrigatório.");
            
        if (address.Length < 5 || address.Length > 200)
            throw new StudentDomainException("Endereço deve ter entre 5 e 200 caracteres.");
            
        if (string.IsNullOrWhiteSpace(city))
            throw new StudentDomainException("Cidade é obrigatória.");
            
        if (city.Length < 2 || city.Length > 100)
            throw new StudentDomainException("Cidade deve ter entre 2 e 100 caracteres.");
            
        if (string.IsNullOrWhiteSpace(state))
            throw new StudentDomainException("Estado é obrigatório.");
            
        if (state.Length != 2)
            throw new StudentDomainException("Estado deve ter 2 caracteres.");
            
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new StudentDomainException("CEP é obrigatório.");
            
        // Remove hífen do CEP se presente e valida se tem 8 dígitos
        var cleanZipCode = zipCode.Replace("-", "");
        if (cleanZipCode.Length != 8 || !cleanZipCode.All(char.IsDigit))
            throw new StudentDomainException("CEP deve ter 8 dígitos. Formato aceito: 12345678 ou 12345-678.");
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var normalized = email.Trim();
            var addr = new System.Net.Mail.MailAddress(normalized);
            if (addr.Address != normalized)
            {
                return false;
            }

            // Exigir domínio com ponto e TLD com pelo menos 2 caracteres
            var parts = normalized.Split('@');
            if (parts.Length != 2)
            {
                return false;
            }

            var domain = parts[1];
            var lastDot = domain.LastIndexOf('.');
            if (lastDot <= 0 || lastDot == domain.Length - 1)
            {
                return false;
            }

            var tld = domain.Substring(lastDot + 1);
            if (tld.Length < 2)
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
} 