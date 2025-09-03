using FluentValidation;
using SISGED.Shared.DTOs;
using System.Net.Http.Json;

namespace SISGED.Shared.Validators;

public class SessionResolutionValidator : AbstractValidator<SessionResolutionRegisterDTO>
{
    private readonly HttpClient _httpClient;

    public SessionResolutionValidator(HttpClient httpClient)
    {
        _httpClient = httpClient;

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Debe ingresar el título de la resolución de la sesión")
            .MustAsync(async (title, _) =>
            {
                if (string.IsNullOrEmpty(title))
                    return false;

                var isTitleUnique = await ValidateTitleAsync(title);

                return isTitleUnique;
            })
            .WithMessage("El título de la resolución de la sesión ya se encuentra registrado");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Debe ingresar la descripción de la resolución de la sesión");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<SessionResolutionRegisterDTO>
            .CreateWithOptions((SessionResolutionRegisterDTO)model, x => x.IncludeProperties(propertyName)));

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