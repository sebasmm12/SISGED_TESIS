using System.Net.Http.Json;
using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators;

public class SolicitorDossierShipmentValidator : AbstractValidator<SolicitorDossierShipmentRegisterDTO>
{
    private readonly HttpClient _httpClient;

    public SolicitorDossierShipmentValidator(HttpClient httpClient)
    {
        _httpClient = httpClient;

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Debe ingresar el título de la entrega del expediente")
            .MustAsync(async (title, _) =>
            {
                if (string.IsNullOrEmpty(title))
                    return false;

                var isTitleUnique = await ValidateTitleAsync(title);

                return isTitleUnique;
            })
            .WithMessage("El título de la entrega del expediente ya se encuentra registrado");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Debe ingresar la conclusión de la entrega del expediente");

        RuleFor(x => x.SolicitorDossiers)
            .NotEmpty()
            .WithMessage("Debe seleccionar al menos un expediente del notario");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<SolicitorDossierShipmentRegisterDTO>
            .CreateWithOptions((SolicitorDossierShipmentRegisterDTO)model, x => x.IncludeProperties(propertyName)));

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