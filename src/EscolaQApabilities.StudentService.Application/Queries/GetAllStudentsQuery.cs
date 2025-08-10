using EscolaQApabilities.StudentService.Application.DTOs;
using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Repositories;
using MediatR;

namespace EscolaQApabilities.StudentService.Application.Queries;

public record GetAllStudentsQuery : IRequest<IEnumerable<StudentDto>>;

public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, IEnumerable<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;

    public GetAllStudentsQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<IEnumerable<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _studentRepository.GetAllAsync();

        return students.Select(student => new StudentDto(
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
            student.UpdatedAt));
    }
} 