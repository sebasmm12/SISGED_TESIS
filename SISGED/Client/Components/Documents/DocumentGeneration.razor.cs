using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.DossierTray;

namespace SISGED.Client.Components.Documents
{
    public partial class DocumentGeneration
    {
        [Inject]
        private DocumentGeneratorStrategy DocumentGeneratorStrategy { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;
        
        private readonly string documentGenerationCreation = DateTime.UtcNow.AddHours(-5).ToString("dd/MM/yyyy");
        private string documentType = "";
        private bool pageLoading = true;

        private GenerateDocumentRequest generateDocumentRequest = default!;
        private RenderFragment ChildContent = default!;
        private DocumentGenerator documentGenerator = default!;
        private IJSObjectReference pdfGeneratorModule = default!;

        private static string GenerateCode(DossierTrayResponse dossierTray, DocumentResponse document)
        {
            string lastDocumentType = document.Type;
            string user = dossierTray.Client!.DocumentNumber;
            long unixTime = GetUnixTime(DateTime.UtcNow.AddHours(-5));
            string serialization = GetSerialization(dossierTray, document);

            string code = lastDocumentType + "-" + user + "-" + unixTime + "-" + serialization;

            return code;

        }

        private void GetDocumentInformation()
        {
            var dossierTray = GetDossierTray();
            var lastDocument = dossierTray.DocumentObjects!.Last();

            generateDocumentRequest = new(lastDocument.Id, dossierTray.DossierId, GenerateCode(dossierTray, lastDocument));
            documentGenerator = new(lastDocument, generateDocumentRequest.Code);

            documentType = lastDocument.Type;
        }

        private void GetSignature(MediaRegisterDTO sign)
        {
            generateDocumentRequest.Sign = sign;
        }

        protected async override Task OnInitializedAsync()
        {
            await Task.Delay(100);

            GetDocumentInformation();
            GetDocumentGenerator();

            pageLoading = false;
        }

        private void GetDocumentGenerator()
        {
            ChildContent = DocumentGeneratorStrategy.GetDocument(documentType).RenderFragment;
        }

        private static string GetSerialization(DossierTrayResponse dossierTray, DocumentResponse lastDocument)
        {

            int totalDocuments = 0;

            foreach (var document in dossierTray.DocumentObjects!)
            {
                if (document.Id == lastDocument.Id) break;

                totalDocuments++;
            }

            return string.Format("{0:000}", totalDocuments);
        }

        private static long GetUnixTime(DateTime dateTime)
        {
            DateTime unixTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            var totalSeconds = (dateTime - unixTime).TotalSeconds;

            return (long)totalSeconds;
        }

        private DossierTrayResponse GetDossierTray()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            var dossierTray = userTray.Value as DossierTrayResponse;

            return dossierTray!;
        }

        private async Task GenerateDocument()
        {
            //pdfGeneratorModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "../js/pdf-generator.js");

            var generatedUrl = await JSRuntime.InvokeAsync<string>("generatePdf", generateDocumentRequest.Sign.Content, "Sebastian Miranda", "Solicitud de Denuncia: 2");

            await ShowPdfPrevisualization(generatedUrl);
        }
        
        private async Task<DialogResult> ShowPdfPrevisualization(string generatedUrl)
        {
            var dialogParameters = GetDialogParameters(new() { new("Url", generatedUrl) });

            var dialogOptions = new DialogOptions { CloseButton = true };

            var dialogService = DialogService.Show<PdfPrevisualization>("Previsualización del documento", dialogParameters, dialogOptions);

            return await dialogService.Result;
        }

        private static DialogParameters GetDialogParameters(List<DialogParameter> dialogParameterDTOs)
        {
            var dialogParameters = new DialogParameters();

            dialogParameterDTOs.ForEach(dialogParameterDTO =>
            {
                dialogParameters.Add(dialogParameterDTO.Name, dialogParameterDTO.Value);
            });

            return dialogParameters;
        }
    }
}