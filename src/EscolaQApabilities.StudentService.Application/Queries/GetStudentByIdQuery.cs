using EscolaQApabilities.StudentService.Application.DTOs;
using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using MediatR;

namespace EscolaQApabilities.StudentService.Application.Queries;

public record GetStudentByIdQuery(Guid Id) : IRequest<StudentDto>;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, StudentDto>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentByIdQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id);
        if (student == null)
        {
            throw new StudentDomainException($"Aluno com ID {request.Id} não encontrado.");
        }

        return new StudentDto(
            student.Id,
            student.Name,
            student.Email,
            student.Phone,
            student.BirthDate,
            student.Address,
            student.City,
            student.State,
            student.ZipCode,
            student.Status,
            student.EnrollmentDate,
            student.ParentName,
            student.ParentPhone,
            student.ParentEmail,
            student.EmergencyContact,
            student.EmergencyPhone,
            student.MedicalInformation,
            student.Notes,
            student.CreatedAt,
            student.UpdatedAt);
    }
} 