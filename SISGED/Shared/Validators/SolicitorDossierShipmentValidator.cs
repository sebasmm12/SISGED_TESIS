using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators
{
    public class SolicitorDossierShipmentValidator : AbstractValidator<SolicitorDossierShipmentRegisterDTO>
    {
        public SolicitorDossierShipmentValidator()
        {
            RuleFor(x => x.Title)
               .NotEmpty()
               .WithMessage("Debe ingresar el título de la entrega del expediente");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la conclusión de la entrega del expediente");

            RuleFor(x => x.SolicitorDossiers)
                .NotEmpty()
                .WithMessage("Debe seleccionar al menos un expediente del notario");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<SolicitorDossierShipmentRegisterDTO>
                    .CreateWithOptions((SolicitorDossierShipmentRegisterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
