using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.SolicitorDossier;
using System.Text.Json;
using Microsoft.JSInterop;

namespace SISGED.Client.Components.Documents.Generators
{
    public partial class SolicitorDossierShipmentGenerator
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [CascadingParameter(Name = "DocumentGenerator")]
        public DocumentGenerator DocumentGenerator { get; set; } = default!;

        private SolicitorDossierShipmentContentDTO solicitorDossierShipmentContent = default!;
        private SolicitorDossierShipmentDTO solicitorDossierShipment = default!;
        private bool canShow = false;

        protected override async Task OnInitializedAsync()
        {
            solicitorDossierShipmentContent = JsonSerializer.Deserialize<SolicitorDossierShipmentContentDTO>(JsonSerializer.Serialize(DocumentGenerator.Document.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            await GetSolicitorDossierShipmentInfoAsync();

            canShow = true;
        }

        private async Task GetSolicitorDossierShipmentInfoAsync()
        {
            var solicitorTask = GetSolicitorAsync(solicitorDossierShipmentContent.SolicitorId);
            var solicitorDossierTask = GetSolicitorDossiersAsync(solicitorDossierShipmentContent.SolicitorDossiers!);

            await Task.WhenAll(solicitorTask, solicitorDossierTask);

            var solicitor = await solicitorTask;
            var solicitorDossier = await solicitorDossierTask;

            solicitorDossierShipment = new(solicitor, solicitorDossier);
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

        private async Task<IEnumerable<SolicitorDossierByIdsResponse>> GetSolicitorDossiersAsync(IEnumerable<string> solicitorDossiersIds)
        {
            try
            {
                string solicitorDossierQueries = GetSolicitorDossierQueries(solicitorDossiersIds);

                var solicitorDossiersResponse = await HttpRepository.GetAsync<IEnumerable<SolicitorDossierByIdsResponse>>($"api/solicitorsDossiers/identifiers{solicitorDossierQueries}");

                if(solicitorDossiersResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los expedientes del notario");
                }

                return solicitorDossiersResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los expedientes del notario");
                return new List<SolicitorDossierByIdsResponse>();
            }
        }

        private static string GetSolicitorDossierQueries(IEnumerable<string> solicitorDossiersIds)
        {
            string solicitorDossierQueries = "?";

            solicitorDossierQueries += solicitorDossiersIds.Aggregate((first, second) => $"&solicitorDossierIds={first}&solicitorDossierIdS={second}");

            return solicitorDossierQueries;
        }
    }
}