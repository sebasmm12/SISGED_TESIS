using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
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
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;

        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;
        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private readonly string documentGenerationCreation = DateTime.UtcNow.AddHours(-5).ToString("dd/MM/yyyy");
        private string documentType = "";
        private bool pageLoading = true;
        private bool canGenerate = false;

        private GenerateDocumentRequest generateDocumentRequest = default!;
        private RenderFragment ChildContent = default!;
        private DocumentGenerator documentGenerator = default!;
        private IJSObjectReference pdfGeneratorModule = default!;
        private DossierTrayResponse dossierTray = default!;

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
            dossierTray = GetDossierTray();
            var lastDocument = dossierTray.DocumentObjects!.Last();

            generateDocumentRequest = new(lastDocument.Id, dossierTray.DossierId, GenerateCode(dossierTray, lastDocument));
            documentGenerator = new(lastDocument, generateDocumentRequest.Code, dossierTray);

            documentType = lastDocument.Type;
        }

        private async Task GetSignatureAsync(MediaRegisterDTO sign)
        {
            generateDocumentRequest.Sign = sign;
            canGenerate = true;

            await WorkEnvironment.UpdateAssistantMessageAsync(new(dossierTray.Type!, dossierTray.Document!.Type, 2));
        }

        protected override async Task OnInitializedAsync()
        {
            GetDocumentInformation();
            GetDocumentGenerator();

            await WorkEnvironment.UpdateAssistantMessageAsync(new(dossierTray.Type!, dossierTray.Document!.Type, 1));

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
            pdfGeneratorModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "../js/pdf-generator.js");
            
            string user = SessionAccount.GetUser().GetFullName();
            string userDocument = $"{SessionAccount.GetDocumentType().ToUpper()} : {SessionAccount.GetDocumentNumber()}";

            var generatedUrl = await pdfGeneratorModule.InvokeAsync<string>("generatePdf", generateDocumentRequest.Sign, user, userDocument);
      
            generateDocumentRequest.GeneratedURL = GetGeneratedUrl(generatedUrl);

            var result = await ShowPdfPrevisualization(generatedUrl);

            if (result.Cancelled) return;

            var document = await ShowLoadingDialogAsync(generateDocumentRequest);

            await SwalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo generar la denuncia de manera satisfactoria");

            await UpdateGeneratedDocumentAsync(document!);
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

        private async Task<Document?> GenerateDocumentAsync(GenerateDocumentRequest generateDocumentRequest)
        {
            try
            {
                var complaintResponse = await HttpRepository.PutAsync<GenerateDocumentRequest, Document>("api/documents/generation", generateDocumentRequest);

                if(complaintResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo generar la solicitud de denuncia");
                }

                return complaintResponse.Response;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo generar la solicitud de denuncia");
                return null;
            }
        }

        private async Task<Document?> ShowLoadingDialogAsync(GenerateDocumentRequest generateDocumentRequest)
        {
            string dialogTitle = $"Realizando la generación del documento, por favor espere...";

            var complaintToRegister = () => GenerateDocumentAsync(generateDocumentRequest);

            return await DialogContentRepository.ShowLoadingDialogAsync(complaintToRegister, dialogTitle);

        }

        private static MediaRegisterDTO GetGeneratedUrl(string url)
        {

            var urlParts = url.Split(";");

            string fileName = urlParts.ElementAt(1);
            string extension = Path.GetExtension(fileName);
            string content = urlParts.Last().Split(",").Last();

            return new(content, extension, fileName);
        }

        private async Task UpdateGeneratedDocumentAsync(Document generatedDocument)
        {
            var item = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace != "tools");

            ProcessWorkItemInfo(item!, generatedDocument);
            
            await WorkEnvironment.UpdateGeneratedDocumentAsync(item!);
        }

        private void ProcessWorkItemInfo(Helpers.Item item, Document generatedDocument)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var documentResponse = Mapper.Map<DocumentResponse>(generatedDocument);

            dossierTray.Document = documentResponse;
            dossierTray.DocumentObjects!.RemoveAt(dossierTray.DocumentObjects.Count - 1);
            dossierTray.DocumentObjects.Add(dossierTray.Document);

            Mapper.Map(dossierTray, item);
        }
    }
}