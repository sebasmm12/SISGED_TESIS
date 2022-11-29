using FluentValidation;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Shared.Validators
{
    public class DisciplinaryOpennessRegisterValidator : AbstractValidator<DisciplinaryOpennessResponse>
    {
        public DisciplinaryOpennessRegisterValidator()
        {
            RuleFor(x => x.Content.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título de la solicitud");

            RuleFor(x => x.Content.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la descripción de la solicitud");
            
            RuleFor(x => x.Content.Complainant)
                .NotEmpty()
                .WithMessage("Debe ingresar el nombre del denunciante");
            
            RuleFor(x => x.Content.AudienceLocation)
                .NotEmpty()
                .WithMessage("Debe ingresar el lugar de la audiencia");

            RuleFor(x => x.Content.AudienceStartDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de inicio de audiencia");

            RuleFor(x => x.Content.AudienceEndDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de fin de audiencia");

            RuleFor(x => x.Content.SolicitorId)
                .NotEmpty()
                .WithMessage("Debe ingresar el nombre del notario");

            RuleFor(x => x.Content.ProsecutorId)
                .NotEmpty()
                .WithMessage("Debe ingresar el nombre del fiscal");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DisciplinaryOpennessResponse>
                    .CreateWithOptions((DisciplinaryOpennessResponse)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
