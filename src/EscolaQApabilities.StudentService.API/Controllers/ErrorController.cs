using EscolaQApabilities.StudentService.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EscolaQApabilities.StudentService.API.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    [HttpGet]
    [Route("/error")]
    public IActionResult Error()
    {
        var exception = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        if (exception is StudentDomainException domainException)
        {
            return BadRequest(new
            {
                Error = "Erro de Domínio",
                Message = domainException.Message
            });
        }

        return StatusCode(500, new
        {
            Error = "Erro Interno do Servidor",
            Message = "Ocorreu um erro inesperado. Tente novamente mais tarde."
        });
    }
} 