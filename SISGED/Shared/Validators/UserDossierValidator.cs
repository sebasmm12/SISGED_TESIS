using FluentValidation;
using SISGED.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Validators
{
    public class UserDossierValidator : AbstractValidator<UserDossierFilterDTO>
    {
        public UserDossierValidator()
        {

            When(x => x.StartDate.HasValue && x.EndDate.HasValue, () =>
            {
                RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("La fecha de inicio debe ser mayor a la fecha fin");
            });
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserDossierFilterDTO>
                    .CreateWithOptions((UserDossierFilterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };
    }
}
