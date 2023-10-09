using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Solicitor;
using System.Text.Json;

namespace SISGED.Client.Components.Documents.Generators
{
    public partial class SessionResolutionGenerator
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [CascadingParameter(Name = "DocumentGenerator")]
        public DocumentGenerator DocumentGenerator { get; set; } = default!;


        private SessionResolutionContentDTO sessionResolutionContent = default!;
        private SessionResolutionDTO sessionResolution = default!;
        private bool canShow = false;

        protected override async Task OnInitializedAsync()
        {
            sessionResolutionContent = JsonSerializer.Deserialize<SessionResolutionContentDTO>(JsonSerializer.Serialize(DocumentGenerator.Document.Content), 
                                                                                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            await GetSessionResolutionInfoAsync();

            canShow = true;
        }

        private async Task GetSessionResolutionInfoAsync()
        {
            var solicitor = await GetSolicitorAsync(sessionResolutionContent.SolicitorId);

            var previousDocument = DocumentGenerator
                                        .Dossier
                                        .DocumentObjects!
                                        .FirstOrDefault(documentObject => documentObject.Id == sessionResolutionContent.PreviousDocumentId);

            var previousDocumentTitle = JsonSerializer.Deserialize<DocumentContentDTO>(JsonSerializer.Serialize(previousDocument!.Content),
                                                                                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!; ;

            sessionResolution = new(DocumentGenerator.Dossier.Client!, solicitor, previousDocumentTitle);
        }

        private async Task<AutocompletedSolicitorResponse> GetSolicitorAsync(string solicitorId)
        {
            try
            {
                var solicitorResponse = await HttpRepository.GetAsync<AutocompletedSolicitorResponse>($"api/solicitors/{solicitorId}");

                if (solicitorResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                }

                return solicitorResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                return new();
            }
        }
    }
}
