using EscolaQApabilities.StudentService.Application.DTOs;
using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using MediatR;

namespace EscolaQApabilities.StudentService.Application.Commands;

public record CreateStudentCommand(CreateStudentDto StudentDto) : IRequest<StudentDto>;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;

    public CreateStudentCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentDto> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.StudentDto;

        // Verificar se já existe um aluno com o mesmo email
        var existingStudent = await _studentRepository.GetByEmailAsync(dto.Email);
        if (existingStudent != null)
        {
            throw new StudentDomainException($"Já existe um aluno cadastrado com o email {dto.Email}.");
        }

        // Criar nova entidade Student
        var student = new Student(
            dto.Name,
            dto.Email,
            dto.Phone,
            dto.BirthDate,
            dto.Address,
            dto.City,
            dto.State,
            dto.ZipCode,
            dto.ParentName,
            dto.ParentPhone,
            dto.ParentEmail,
            dto.EmergencyContact,
            dto.EmergencyPhone,
            dto.MedicalInformation,
            dto.Notes);

        // Salvar no repositório
        var savedStudent = await _studentRepository.AddAsync(student);

        // Retornar DTO
        return new StudentDto(
            savedStudent.Id,
            savedStudent.Name,
            savedStudent.Email,
            savedStudent.Phone,
            savedStudent.BirthDate,
            savedStudent.Address,
            savedStudent.City,
            savedStudent.State,
            savedStudent.ZipCode,
            savedStudent.Status,
            savedStudent.EnrollmentDate,
            savedStudent.ParentName,
            savedStudent.ParentPhone,
            savedStudent.ParentEmail,
            savedStudent.EmergencyContact,
            savedStudent.EmergencyPhone,
            savedStudent.MedicalInformation,
            savedStudent.Notes,
            savedStudent.CreatedAt,
            savedStudent.UpdatedAt);
    }
} 