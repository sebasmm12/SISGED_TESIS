using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Shared.DTOs;
using Entities = SISGED.Shared.Entities;
using System.Text.Json;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Components.Documents.Generators
{
    public partial class ComplaintRequestGenerator
    {

        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [CascadingParameter(Name = "DocumentGenerator")]
        public DocumentGenerator DocumentGenerator { get; set; } = default!;

        private ComplaintRequestContentDTO complaintRequestContent = default!;
        private ComplaintRequestDTO complaintRequest = default!;
        private bool canShow = false;

        protected override async Task OnInitializedAsync()
        {
            complaintRequestContent = JsonSerializer.Deserialize<ComplaintRequestContentDTO>(JsonSerializer.Serialize(DocumentGenerator.Document.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            await GetComplaintRequestInfoAsync();

            canShow = true;
        }

        private async Task GetComplaintRequestInfoAsync()
        {
            var solicitorTask =  GetSolicitorAsync(complaintRequestContent.SolicitorId);
            var documentTypeTask =  GetDocumentTypeAsync(complaintRequestContent.ComplaintType);

            await Task.WhenAll(solicitorTask, documentTypeTask);

            var solicitor = await solicitorTask;
            var documentType = await documentTypeTask;

            complaintRequest = new(DocumentGenerator.Dossier.Client! , solicitor, documentType);
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

        private async Task<DocumentTypeInfoResponse> GetDocumentTypeAsync(string documentTypeId)
        {
            try
            {
                var documentTypesResponse = await HttpRepository.GetAsync<DocumentTypeInfoResponse>($"api/documentTypes/{documentTypeId}");

                if (documentTypesResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                }

                return documentTypesResponse.Response!;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                return new();
            }
        }
    }
}