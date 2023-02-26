using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment;

namespace SISGED.Client.Components.Documents.Informations
{
    public partial class SolicitorDossierShipmentInfo
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        private IAnnexFactory AnnexFactory { get; set; } = default!;

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; } = default!;

        [Parameter]
        public string DocumentId { get; set; } = default!;

        private bool pageLoading = true;
        private SolicitorDossierShipmentInfoResponse? document;

        protected override async Task OnInitializedAsync()
        {
            document = await GetDocumentAsync(DocumentId);

            pageLoading = false;
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private AnnexPreview GetDocumentPreview(string documentUrl)
        {
            var extension = Path.GetExtension(documentUrl);

            var documentPreview = AnnexFactory.GetAnnexPreview(extension);

            return documentPreview;
        }

        private async Task<SolicitorDossierShipmentInfoResponse?> GetDocumentAsync(string documentId)
        {
            try
            {
                var documentResponse = await HttpRepository.GetAsync<SolicitorDossierShipmentInfoResponse>($"api/documents/solicitorDossierShipments/{documentId}");

                if (documentResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener la información del documento");
                }

                return documentResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener la información del documento");
                return null;
            }
        }
    }
}