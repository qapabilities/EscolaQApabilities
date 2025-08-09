using EscolaQApabilities.StudentService.Application.Commands;
using EscolaQApabilities.StudentService.Application.DTOs;
using EscolaQApabilities.StudentService.Application.Queries;
using EscolaQApabilities.StudentService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EscolaQApabilities.StudentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "TeacherOrAdmin")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todos os alunos (apenas Admin)
    /// </summary>
    /// <returns>Lista de alunos</returns>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(IEnumerable<StudentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
    {
        var query = new GetAllStudentsQuery();
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Cria um novo aluno (apenas Admin)
    /// </summary>
    /// <param name="createStudentDto">Dados do aluno a ser criado</param>
    /// <returns>Aluno criado</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto createStudentDto)
    {
        var command = new CreateStudentCommand(createStudentDto);
        var result = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Busca um aluno por ID
    /// </summary>
    /// <param name="id">ID do aluno</param>
    /// <returns>Dados do aluno</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> GetById(Guid id)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Busca alunos com filtros e paginação
    /// </summary>
    /// <param name="searchTerm">Termo de busca</param>
    /// <param name="status">Status do aluno</param>
    /// <param name="page">Página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de alunos</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(StudentListDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StudentListDto>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] StudentStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var searchDto = new StudentSearchDto(searchTerm, status, page, pageSize);
        var query = new SearchStudentsQuery(searchDto);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Atualiza dados pessoais de um aluno
    /// </summary>
    /// <param name="id">ID do aluno</param>
    /// <param name="updateStudentDto">Novos dados do aluno</param>
    /// <returns>Aluno atualizado</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentDto>> Update(Guid id, [FromBody] UpdateStudentDto updateStudentDto)
    {
        var command = new UpdateStudentCommand(id, updateStudentDto);
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    /// <summary>
    /// Atualiza informações de contato de um aluno
    /// </summary>
    /// <param name="id">ID do aluno</param>
    /// <param name="contactDto">Novas informações de contato</param>
    /// <returns>Aluno atualizado</returns>
    [HttpPut("{id:guid}/contact")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> UpdateContact(Guid id, [FromBody] UpdateStudentContactDto contactDto)
    {
        var command = new UpdateStudentContactCommand(id, contactDto);
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    /// <summary>
    /// Atualiza o status de um aluno
    /// </summary>
    /// <param name="id">ID do aluno</param>
    /// <param name="status">Novo status</param>
    /// <returns>Aluno atualizado</returns>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentDto>> UpdateStatus(Guid id, [FromBody] StudentStatus status)
    {
        var command = new UpdateStudentStatusCommand(id, status);
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    /// <summary>
    /// Remove um aluno (apenas Admin)
    /// </summary>
    /// <param name="id">ID do aluno</param>
    /// <returns>Confirmação da remoção</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var command = new DeleteStudentCommand(id);
        await _mediator.Send(command);
        
        return NoContent();
    }
} 