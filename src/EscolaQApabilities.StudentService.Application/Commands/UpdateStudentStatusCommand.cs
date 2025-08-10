using EscolaQApabilities.StudentService.Application.DTOs;
using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using EscolaQApabilities.StudentService.Domain.Enums;
using MediatR;

namespace EscolaQApabilities.StudentService.Application.Commands;

public record UpdateStudentStatusCommand(Guid Id, StudentStatus Status) : IRequest<StudentDto>;

public class UpdateStudentStatusCommandHandler : IRequestHandler<UpdateStudentStatusCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentStatusCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentDto> Handle(UpdateStudentStatusCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id);
        if (student == null)
        {
            throw new StudentDomainException($"Aluno com ID {request.Id} não encontrado.");
        }

        // Atualizar status baseado no comando
        switch (request.Status)
        {
            case StudentStatus.Active:
                student.Activate();
                break;
            case StudentStatus.Inactive:
                student.Deactivate();
                break;
            case StudentStatus.Suspended:
                student.Suspend();
                break;
            default:
                throw new StudentDomainException($"Status {request.Status} não é válido.");
        }

        // Salvar no repositório
        var updatedStudent = await _studentRepository.UpdateAsync(student);

        // Retornar DTO
        return new StudentDto(
            updatedStudent.Id,
            updatedStudent.Name,
            updatedStudent.Email,
            updatedStudent.Phone,
            updatedStudent.BirthDate,
            updatedStudent.Address,
            updatedStudent.City,
            updatedStudent.State,
            updatedStudent.ZipCode,
            updatedStudent.Status,
            updatedStudent.EnrollmentDate,
            updatedStudent.ParentName,
            updatedStudent.ParentPhone,
            updatedStudent.ParentEmail,
            updatedStudent.EmergencyContact,
            updatedStudent.EmergencyPhone,
            updatedStudent.MedicalInformation,
            updatedStudent.Notes,
            updatedStudent.CreatedAt,
            updatedStudent.UpdatedAt);
    }
} 