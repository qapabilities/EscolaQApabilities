using EscolaQApabilities.StudentService.Domain.Repositories;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using MediatR;

namespace EscolaQApabilities.StudentService.Application.Commands;

public record DeleteStudentCommand(Guid Id) : IRequest<bool>;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public DeleteStudentCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<bool> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id);
        if (student == null)
        {
            throw new StudentDomainException($"Aluno com ID {request.Id} não encontrado.");
        }

        await _studentRepository.DeleteAsync(request.Id);
        return true;
    }
} 