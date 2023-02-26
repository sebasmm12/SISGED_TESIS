using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Document.Resolution;

namespace SISGED.Client.Components.Documents.Informations
{
    public partial class ResolutionInfo
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
        private ResolutionInfoResponse? document;


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

        private async Task<ResolutionInfoResponse?> GetDocumentAsync(string documentId)
        {
            try
            {
                var documentResponse = await HttpRepository.GetAsync<ResolutionInfoResponse>($"api/documents/resolutions/{documentId}");

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