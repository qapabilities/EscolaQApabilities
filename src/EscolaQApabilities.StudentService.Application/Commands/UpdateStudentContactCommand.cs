using EscolaQApabilities.StudentService.Application.DTOs;
using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using MediatR;

namespace EscolaQApabilities.StudentService.Application.Commands;

public record UpdateStudentContactCommand(Guid Id, UpdateStudentContactDto ContactDto) : IRequest<StudentDto>;

public class UpdateStudentContactCommandHandler : IRequestHandler<UpdateStudentContactCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentContactCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentDto> Handle(UpdateStudentContactCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id);
        if (student == null)
        {
            throw new StudentDomainException($"Aluno com ID {request.Id} não encontrado.");
        }

        var dto = request.ContactDto;

        // Atualizar informações de contato
        student.UpdateContactInfo(
            dto.ParentName,
            dto.ParentPhone,
            dto.ParentEmail,
            dto.EmergencyContact,
            dto.EmergencyPhone);

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