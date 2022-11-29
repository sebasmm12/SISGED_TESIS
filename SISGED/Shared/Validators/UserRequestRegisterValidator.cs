using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators
{
    public class UserRequestRegisterValidator: AbstractValidator<UserRequestRegisterDTO>
    {

        public UserRequestRegisterValidator()
        {
            RuleFor(x => x.DocumentType)
                .NotNull()
                .WithMessage("Debe seleccionar un tipo de solicitud");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título de la solicitud");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la descripción de la solicitud");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserRequestRegisterDTO>
                    .CreateWithOptions((UserRequestRegisterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
