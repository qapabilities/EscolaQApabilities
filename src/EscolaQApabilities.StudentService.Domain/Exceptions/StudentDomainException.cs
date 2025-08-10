namespace EscolaQApabilities.StudentService.Domain.Exceptions;

public class StudentDomainException : Exception
{
    public StudentDomainException(string message) : base(message)
    {
    }

    public StudentDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 