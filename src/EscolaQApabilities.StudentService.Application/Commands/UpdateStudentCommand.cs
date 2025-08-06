using EscolaQApabilities.StudentService.Application.DTOs;
using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using MediatR;

namespace EscolaQApabilities.StudentService.Application.Commands;

public record UpdateStudentCommand(Guid Id, UpdateStudentDto StudentDto) : IRequest<StudentDto>;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentDto> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id);
        if (student == null)
        {
            throw new StudentDomainException($"Aluno com ID {request.Id} não encontrado.");
        }

        var dto = request.StudentDto;

        // Verificar se o email já está sendo usado por outro aluno
        var existingStudent = await _studentRepository.GetByEmailAsync(dto.Email);
        if (existingStudent != null && existingStudent.Id != request.Id)
        {
            throw new StudentDomainException($"Já existe um aluno cadastrado com o email {dto.Email}.");
        }

        // Atualizar dados pessoais
        student.UpdatePersonalInfo(
            dto.Name,
            dto.Email,
            dto.Phone,
            dto.BirthDate,
            dto.Address,
            dto.City,
            dto.State,
            dto.ZipCode);

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