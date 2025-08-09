using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Enums;
using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EscolaQApabilities.StudentService.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly StudentDbContext _context;

    public StudentRepository(StudentDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Email == email.ToLower());
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetByStatusAsync(StudentStatus status)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.Status == status)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.Name.ToLower().Contains(term) ||
                       s.Email.ToLower().Contains(term) ||
                       s.City.ToLower().Contains(term) ||
                       s.State.ToLower().Contains(term) ||
                       (s.ParentName != null && s.ParentName.ToLower().Contains(term)))
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Students
            .AsNoTracking()
            .AnyAsync(s => s.Id == id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Students
            .AsNoTracking()
            .AnyAsync(s => s.Email == email.ToLower());
    }

    public async Task<Student> AddAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task DeleteAsync(Guid id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountAsync()
    {
        return await _context.Students.CountAsync();
    }

    public async Task<IEnumerable<Student>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.Students
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> SearchPagedAsync(string? searchTerm, StudentStatus? status, int page, int pageSize)
    {
        IQueryable<Student> query = _context.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(term)
                                  || s.Email.ToLower().Contains(term)
                                  || s.City.ToLower().Contains(term)
                                  || s.State.ToLower().Contains(term)
                                  || (s.ParentName != null && s.ParentName.ToLower().Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var students = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students, totalCount);
    }
} 