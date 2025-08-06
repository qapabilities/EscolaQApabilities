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
        IEnumerable<Student> students;

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm) && searchDto.Status.HasValue)
        {
            // Buscar por termo e status
            var searchResults = await _studentRepository.SearchAsync(searchDto.SearchTerm);
            var statusResults = await _studentRepository.GetByStatusAsync(searchDto.Status.Value);
            students = searchResults.Intersect(statusResults, new StudentComparer());
        }
        else if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
        {
            // Buscar apenas por termo
            students = await _studentRepository.SearchAsync(searchDto.SearchTerm);
        }
        else if (searchDto.Status.HasValue)
        {
            // Buscar apenas por status
            students = await _studentRepository.GetByStatusAsync(searchDto.Status.Value);
        }
        else
        {
            // Buscar todos
            students = await _studentRepository.GetAllAsync();
        }

        // Aplicar paginação
        var totalCount = students.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);
        var pagedStudents = students
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize);

        var studentDtos = pagedStudents.Select(student => new StudentDto(
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

// Comparador para interseção de coleções
public class StudentComparer : IEqualityComparer<Student>
{
    public bool Equals(Student? x, Student? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Id.Equals(y.Id);
    }

    public int GetHashCode(Student obj)
    {
        return obj.Id.GetHashCode();
    }
} 