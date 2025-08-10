using EscolaQApabilities.StudentService.Application.DTOs;
using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Repositories;
using MediatR;

namespace EscolaQApabilities.StudentService.Application.Queries;

public record SearchStudentsQuery(StudentSearchDto SearchDto) : IRequest<StudentListDto>;

public class SearchStudentsQueryHandler : IRequestHandler<SearchStudentsQuery, StudentListDto>
{
    private readonly IStudentRepository _studentRepository;

    public SearchStudentsQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentListDto> Handle(SearchStudentsQuery request, CancellationToken cancellationToken)
    {
        var searchDto = request.SearchDto;
        var (students, totalCount) = await _studentRepository.SearchPagedAsync(
            searchDto.SearchTerm,
            searchDto.Status,
            searchDto.Page,
            searchDto.PageSize);

        var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

        var studentDtos = students.Select(student => new StudentDto(
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

        return new StudentListDto(
            studentDtos,
            totalCount,
            searchDto.Page,
            searchDto.PageSize,
            totalPages);
    }
}