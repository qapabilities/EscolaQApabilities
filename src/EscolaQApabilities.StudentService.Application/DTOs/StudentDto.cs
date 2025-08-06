using EscolaQApabilities.StudentService.Domain.Enums;

namespace EscolaQApabilities.StudentService.Application.DTOs;

public record StudentDto(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateTime BirthDate,
    string Address,
    string City,
    string State,
    string ZipCode,
    StudentStatus Status,
    DateTime EnrollmentDate,
    string? ParentName,
    string? ParentPhone,
    string? ParentEmail,
    string? EmergencyContact,
    string? EmergencyPhone,
    string? MedicalInformation,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateStudentDto(
    string Name,
    string Email,
    string Phone,
    DateTime BirthDate,
    string Address,
    string City,
    string State,
    string ZipCode,
    string? ParentName,
    string? ParentPhone,
    string? ParentEmail,
    string? EmergencyContact,
    string? EmergencyPhone,
    string? MedicalInformation,
    string? Notes);

public record UpdateStudentDto(
    string Name,
    string Email,
    string Phone,
    DateTime BirthDate,
    string Address,
    string City,
    string State,
    string ZipCode);

public record UpdateStudentContactDto(
    string? ParentName,
    string? ParentPhone,
    string? ParentEmail,
    string? EmergencyContact,
    string? EmergencyPhone);

public record UpdateStudentMedicalDto(string? MedicalInformation);

public record UpdateStudentNotesDto(string? Notes);

public record StudentSearchDto(
    string? SearchTerm,
    StudentStatus? Status,
    int Page = 1,
    int PageSize = 10);

public record StudentListDto(
    IEnumerable<StudentDto> Students,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages); 