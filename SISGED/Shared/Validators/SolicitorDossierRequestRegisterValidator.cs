using System.Net.Http.Json;
using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators;

public class SolicitorDossierRequestRegisterValidator : AbstractValidator<SolicitorDossierRequestRegisterDTO>
{
    private readonly HttpClient _httpClient;

    public SolicitorDossierRequestRegisterValidator(HttpClient httpClient)
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

        RuleFor(x => x.Solicitor)
            .NotEmpty()
            .WithMessage("Debe ingresar el nombre del notario");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<SolicitorDossierRequestRegisterDTO>
            .CreateWithOptions((SolicitorDossierRequestRegisterDTO)model, x => x.IncludeProperties(propertyName)));

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