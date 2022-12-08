using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators
{
    public class ComplaintRequestValidator : AbstractValidator<ComplaintRequestRegisterDTO>
    {
        public ComplaintRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título de la denuncia");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la descripción de la denuncia");

            RuleFor(x => x.Solicitor)
                .NotNull()
                .WithMessage("Debe ingresar al notario denunciado");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ComplaintRequestRegisterDTO>
                    .CreateWithOptions((ComplaintRequestRegisterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
