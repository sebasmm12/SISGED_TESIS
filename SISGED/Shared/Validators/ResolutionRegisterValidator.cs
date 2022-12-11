using FluentValidation;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Validators
{
    public class ResolutionRegisterValidator : AbstractValidator<ResolutionRegisterDTO>
    {
        public ResolutionRegisterValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título de la resolución");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la descripción de la resolución");

            RuleFor(x => x.Penalty)
                .NotEmpty()
                .WithMessage("Debe ingresar la penalidad");

            RuleFor(x => x.Participants)
                .NotEmpty()
                .WithMessage("Debe ingresar los participantes de la resolución");

            RuleFor(x => x.AudienceStartDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de inicio de audiencia");

            RuleFor(x => x.AudienceEndDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de fin de audiencia");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ResolutionRegisterDTO>
                    .CreateWithOptions((ResolutionRegisterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
