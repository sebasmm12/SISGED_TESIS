using FluentValidation;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Dossier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Validators
{
    public class SolicitorDossierRequestRegisterValidator : AbstractValidator<SolicitorDossierRequestResponse>
    {
        public SolicitorDossierRequestRegisterValidator()
        {
            RuleFor(x => x.Content.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título de la solicitud");

            RuleFor(x => x.Content.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la descripción de la solicitud");

            RuleFor(x => x.Content.SolicitorId)
                .NotEmpty()
                .WithMessage("Debe ingresar el nombre del notario");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<SolicitorDossierRequestResponse>
                    .CreateWithOptions((SolicitorDossierRequestResponse)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
