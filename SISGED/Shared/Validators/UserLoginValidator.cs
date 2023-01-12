using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators
{
    public class UserLoginValidator : AbstractValidator<UserLoginDTO>
    {

        public UserLoginValidator()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .WithMessage("Debe ingresar el nombre de usuario-");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Debe ingresar la contraseña.");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserLoginDTO>
                    .CreateWithOptions((UserLoginDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
