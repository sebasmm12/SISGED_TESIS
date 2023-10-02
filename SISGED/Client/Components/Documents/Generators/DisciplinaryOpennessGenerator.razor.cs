using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Solicitor;
using System.Text.Json;

namespace SISGED.Client.Components.Documents.Generators
{
    public partial class DisciplinaryOpennessGenerator
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [CascadingParameter(Name = "DocumentGenerator")]
        public DocumentGenerator DocumentGenerator { get; set; } = default!;

        private DisciplinaryOpennessContentDTO disciplinaryOpennessContent = default!;
        private DisciplinaryOpennessDTO disciplinaryOpenness = default!;
        private bool canShow = false;

        protected override async Task OnInitializedAsync()
        {
            disciplinaryOpennessContent = JsonSerializer.Deserialize<DisciplinaryOpennessContentDTO>(JsonSerializer.Serialize(DocumentGenerator.Document.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            await GetDisciplinaryOpennessInfoAsync();

            canShow = true;
        }

        private async Task GetDisciplinaryOpennessInfoAsync()
        {
            var solicitor = await GetSolicitorAsync(disciplinaryOpennessContent.SolicitorId);

            disciplinaryOpenness = new(DocumentGenerator.Dossier.Client!, solicitor);
        }

        private async Task<AutocompletedSolicitorResponse> GetSolicitorAsync(string solicitorId)
        {
            try
            {
                var solicitorResponse = await HttpRepository.GetAsync<AutocompletedSolicitorResponse>($"api/solicitors/{solicitorId}");

                if (solicitorResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener la información del notario");
                }

                return solicitorResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener la información del notario");
                return new();
            }
        }
    }
}