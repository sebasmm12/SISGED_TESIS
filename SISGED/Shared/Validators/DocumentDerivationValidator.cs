using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators
{
    public class DocumentDerivationValidator : AbstractValidator<DocumentDerivationDTO>
    {
        public DocumentDerivationValidator()
        {
            RuleFor(x => x.UserTray)
                .NotNull()
                .WithMessage("Debe seleccionar el trabajador que recibirá el documento");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DocumentDerivationDTO>
                    .CreateWithOptions((DocumentDerivationDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
