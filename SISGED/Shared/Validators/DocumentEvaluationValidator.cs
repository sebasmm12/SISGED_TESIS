using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators
{
    public class DocumentEvaluationValidator : AbstractValidator<DocumentEvaluationDTO>
    {
        public DocumentEvaluationValidator()
        {
            RuleFor(x => x.EvaluatorComment)
                .NotEmpty()
                .WithMessage("Debe ingresar su comentario de rechazo");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DocumentEvaluationDTO>
                    .CreateWithOptions((DocumentEvaluationDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
