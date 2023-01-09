using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators
{
    public class DictumValidator : AbstractValidator<DictumRegisterDTO>
    {
        public DictumValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título del dictamen");

            RuleFor(x => x.Conclusion)
                .NotEmpty()
                .WithMessage("Debe ingresar la conclusión del dictamen");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DictumRegisterDTO>
                    .CreateWithOptions((DictumRegisterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
