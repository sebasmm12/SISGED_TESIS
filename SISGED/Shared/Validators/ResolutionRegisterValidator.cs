using System.Net.Http.Json;
using FluentValidation;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Validators;

public class ResolutionRegisterValidator : AbstractValidator<ResolutionRegisterDTO>
{
    private readonly HttpClient _httpClient;

    public ResolutionRegisterValidator(HttpClient httpClient)
    {
        _httpClient = httpClient;

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Debe ingresar el título de la resolución")
            .MustAsync(async (title, _) =>
            {
                if (string.IsNullOrEmpty(title))
                    return false;

                var isTitleUnique = await ValidateTitleAsync(title);

                return isTitleUnique;
            })
            .WithMessage("El título de la resolución ya se encuentra registrado");

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
            .NotNull()
            .NotEmpty()
            .WithMessage("Debe ingresar la fecha de inicio de audiencia");

        RuleFor(x => x.AudienceEndDate)
            .NotNull()
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

    private async Task<bool> ValidateTitleAsync(string title)
    {
        var isTitleUnique = await _httpClient
            .GetFromJsonAsync<bool>($"api/documents/validations?title={title}")
            .ConfigureAwait(false);

        return isTitleUnique;
    }
}