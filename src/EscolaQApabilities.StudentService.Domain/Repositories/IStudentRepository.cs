using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Enums;

namespace EscolaQApabilities.StudentService.Domain.Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<Student?> GetByEmailAsync(string email);
    Task<IEnumerable<Student>> GetAllAsync();
    Task<IEnumerable<Student>> GetByStatusAsync(StudentStatus status);
    Task<IEnumerable<Student>> SearchAsync(string searchTerm);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<Student> AddAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task DeleteAsync(Guid id);
    Task<int> CountAsync();
    Task<IEnumerable<Student>> GetPagedAsync(int page, int pageSize);
    Task<(IEnumerable<Student> Students, int TotalCount)> SearchPagedAsync(string? searchTerm, StudentStatus? status, int page, int pageSize);
} 