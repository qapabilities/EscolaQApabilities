using EscolaQApabilities.StudentService.Application.DTOs;
using FluentValidation;

namespace EscolaQApabilities.StudentService.API.Validators;

public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .Matches(@"^\d{10,15}$").WithMessage("Telefone deve conter apenas números entre 10 e 15 dígitos");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Data de nascimento é obrigatória")
            .LessThan(DateTime.Today).WithMessage("Data de nascimento deve ser anterior à data atual")
            .GreaterThan(DateTime.Today.AddYears(-100)).WithMessage("Data de nascimento inválida");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Endereço é obrigatório")
            .Length(5, 200).WithMessage("Endereço deve ter entre 5 e 200 caracteres");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Cidade é obrigatória")
            .Length(2, 100).WithMessage("Cidade deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Cidade deve conter apenas letras e espaços");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("Estado é obrigatório")
            .Length(2, 2).WithMessage("Estado deve ter exatamente 2 caracteres")
            .Matches(@"^[A-Z]{2}$").WithMessage("Estado deve ser a sigla com 2 letras maiúsculas");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("CEP é obrigatório")
            .Length(8, 8).WithMessage("CEP deve ter exatamente 8 dígitos")
            .Matches(@"^\d{8}$").WithMessage("CEP deve conter apenas números");

        // Validações opcionais
        When(x => !string.IsNullOrEmpty(x.ParentName), () =>
        {
            RuleFor(x => x.ParentName)
                .Length(2, 100).WithMessage("Nome do responsável deve ter entre 2 e 100 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome do responsável deve conter apenas letras e espaços");
        });

        When(x => !string.IsNullOrEmpty(x.ParentPhone), () =>
        {
            RuleFor(x => x.ParentPhone)
                .Matches(@"^\d{10,15}$").WithMessage("Telefone do responsável deve conter apenas números entre 10 e 15 dígitos");
        });

        When(x => !string.IsNullOrEmpty(x.ParentEmail), () =>
        {
            RuleFor(x => x.ParentEmail)
                .EmailAddress().WithMessage("Email do responsável deve ter formato válido")
                .MaximumLength(100).WithMessage("Email do responsável deve ter no máximo 100 caracteres");
        });

        When(x => !string.IsNullOrEmpty(x.EmergencyContact), () =>
        {
            RuleFor(x => x.EmergencyContact)
                .Length(2, 100).WithMessage("Contato de emergência deve ter entre 2 e 100 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Contato de emergência deve conter apenas letras e espaços");
        });

        When(x => !string.IsNullOrEmpty(x.EmergencyPhone), () =>
        {
            RuleFor(x => x.EmergencyPhone)
                .Matches(@"^\d{10,15}$").WithMessage("Telefone de emergência deve conter apenas números entre 10 e 15 dígitos");
        });

        When(x => !string.IsNullOrEmpty(x.MedicalInformation), () =>
        {
            RuleFor(x => x.MedicalInformation)
                .MaximumLength(500).WithMessage("Informações médicas devem ter no máximo 500 caracteres");
        });

        When(x => !string.IsNullOrEmpty(x.Notes), () =>
        {
            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Observações devem ter no máximo 1000 caracteres");
        });
    }
}
