using FluentValidation;
using SISGED.Shared.Models.Responses.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Validators
{
    public class ResolutionRegisterValidator : AbstractValidator<ResolutionResponse>
    {
        public ResolutionRegisterValidator()
        {
            RuleFor(x => x.Content.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título de la resolución");

            RuleFor(x => x.Content.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la descripción de la resolución");

            RuleFor(x => x.Content.Penalty)
                .NotEmpty()
                .WithMessage("Debe ingresar la penalidad");

            RuleFor(x => x.Content.Participants)
                .NotEmpty()
                .WithMessage("Debe ingresar los participantes de la resolución");

            RuleFor(x => x.Content.AudienceStartDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de inicio de audiencia");

            RuleFor(x => x.Content.AudienceEndDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de fin de audiencia");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ResolutionResponse>
                    .CreateWithOptions((ResolutionResponse)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
