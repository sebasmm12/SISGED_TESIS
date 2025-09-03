using System.Net.Http.Json;
using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators
{
    public class DisciplinaryOpennessRegisterValidator : AbstractValidator<DisciplinaryOpennessRegisterDTO>
    {
        private readonly HttpClient _httpClient;

        public DisciplinaryOpennessRegisterValidator(HttpClient httpClient)
        {
            _httpClient = httpClient;

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Debe ingresar el título de la solicitud")
                .MustAsync(async (title, _) =>
                {
                    if (string.IsNullOrEmpty(title))
                        return false;

                    var isTitleUnique = await ValidateTitleAsync(title);

                    return isTitleUnique;
                })
                .WithMessage("El título de la solicitud ya se encuentra registrado");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Debe ingresar la descripción de la solicitud");
            
            RuleFor(x => x.AudienceLocation)
                .NotEmpty()
                .WithMessage("Debe ingresar el lugar de la audiencia");

            RuleFor(x => x.AudienceStartDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de inicio de audiencia");
                
            RuleFor(x => x.AudienceEndDate)
                .NotEmpty()
                .WithMessage("Debe ingresar la fecha de fin de audiencia");

            RuleFor(x => x.Solicitor)
                .NotNull()
                .NotEmpty()
                .WithMessage("Debe ingresar el nombre del notario");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DisciplinaryOpennessRegisterDTO>
                    .CreateWithOptions((DisciplinaryOpennessRegisterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };

        private async Task<bool> ValidateTitleAsync(string title)
        {
            var isTitleUnique = await _httpClient.GetFromJsonAsync<bool>($"api/documents/validations?title={title}");

            return isTitleUnique;
        }
    }
}
