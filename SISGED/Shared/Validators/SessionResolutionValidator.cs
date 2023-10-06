using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators
{
    public class SessionResolutionValidator : AbstractValidator<SessionResolutionRegisterDTO>
    {
        public SessionResolutionValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título de la resolución de la sesión");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la descripción de la resolución de la sesión");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<SessionResolutionRegisterDTO>
                .CreateWithOptions((SessionResolutionRegisterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
