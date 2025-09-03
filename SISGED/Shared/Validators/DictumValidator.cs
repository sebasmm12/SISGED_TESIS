using System.Net.Http.Json;
using FluentValidation;
using SISGED.Shared.DTOs;
namespace SISGED.Shared.Validators;

public class DictumValidator : AbstractValidator<DictumRegisterDTO>
{
    private readonly HttpClient _httpClient;

    public DictumValidator(HttpClient httpClient)
    {
        _httpClient = httpClient;

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Debe ingresar el título del dictamen")
            .MustAsync(async (title, _) =>
            {
                if (string.IsNullOrEmpty(title))
                    return false;

                var isTitleUnique = await ValidateTitleAsync(title);

                return isTitleUnique;
            })
            .WithMessage("El título del dictamen ya se encuentra registrado");

        RuleFor(x => x.Conclusion)
            .NotEmpty()
            .WithMessage("Debe ingresar la conclusión del dictamen");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<DictumRegisterDTO>
            .CreateWithOptions((DictumRegisterDTO)model, x => x.IncludeProperties(propertyName)));

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