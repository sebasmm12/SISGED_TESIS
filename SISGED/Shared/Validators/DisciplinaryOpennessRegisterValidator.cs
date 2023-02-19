using FluentValidation;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Shared.Validators
{
    public class DisciplinaryOpennessRegisterValidator : AbstractValidator<DisciplinaryOpennessRegisterDTO>
    {
        public DisciplinaryOpennessRegisterValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título de la solicitud");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la descripción de la solicitud");
            
            RuleFor(x => x.AudienceLocation)
                .NotEmpty()
                .WithMessage("Debe ingresar el lugar de la audiencia");

            RuleFor(x => x.AudienceStartDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de inicio de audiencia");
                
            RuleFor(x => x.AudienceEndDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de fin de audiencia");

            RuleFor(x => x.Solicitor)
                .NotNull()
                .NotEmpty()
                .WithMessage("Debe ingresar el nombre del notario");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DisciplinaryOpennessRegisterDTO>
                    .CreateWithOptions((DisciplinaryOpennessRegisterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
