using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Components.Documents.Histories;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.UserDocument;
using SISGED.Shared.Validators;

namespace SISGED.Client.Pages.Documents
{
    public partial class DocumentsList
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;
        [Inject]
        public IFilterRepository<UserDocumentFilterDTO> UserDocumentRepository { get; set; } = default!;
        [Inject]
        public IDossierRepository DossierRepository { get; set; } = default!;
        [Inject]
        public IDocumentStateRepository DocumentStateRepository { get; set; } = default!;
        [Inject]
        public IDocumentRepository DocumentRepository { get; set; } = default!;
        [Inject]
        public UserDocumentValidator UserDocumentValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;

        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        [Parameter]
        public int PageSize { get; set; } = 5;

        private bool documentsLoading = true;
        private PaginatedUserDocumentDTO paginatedUserDocuments = default!;
        private UserDocumentFilterDTO userDocumentFilter = new();
        private int currentPage = 0;
        private IEnumerable<SelectOption> dossiersTypes = default!;
        private IEnumerable<SelectOption> documentStates = default!;
        private bool documentSearchLoading = false;
        private MudForm? documentSearcherForm = default!;

        private int TotalSolicitorDocuments => (paginatedUserDocuments.Total + PageSize - 1) / PageSize;

        protected override async Task OnInitializedAsync()
        {
            paginatedUserDocuments = await GetDocumentsByUserAsync(SessionAccount.GetUser().Id);

            dossiersTypes = DossierRepository.GetDossierTypes();
            documentStates = DocumentStateRepository.GetDocumentStates();

            documentsLoading = false;
        }
        private async Task ShowDocumentVersionHistoryAsync(UserDocumentDTO document)
        {
            var dialogParameters = new List<DialogParameter>() { new("DocumentId", document.Id), new("PageSize", 5) };

            await DialogContentRepository.ShowDialogAsync<DocumentsVersion>(dialogParameters, "Historial de versiones");
        }


        private async Task ShowDocumentProcessHistoryAsync(UserDocumentDTO document)
        {
            var dialogParameters = new List<DialogParameter>() { new("DocumentId", document.Id), new("PageSize", 5) };

            await DialogContentRepository.ShowDialogAsync<DocumentsProcess>(dialogParameters, "Historial de procesos");
        }

        private async Task ShowDocumentEvaluationAsync(UserDocumentDTO document)
        {
            var dialogParameters = new List<DialogParameter>() { new("DocumentId", document.Id), new("PageSize", 5) };

            await DialogContentRepository.ShowDialogAsync<DocumentsEvaluation>(dialogParameters, "Evaluaciones");

        }

        private async Task ShowDocumentInfoAsync(UserDocumentDTO document)
        {
            var dialogParameters = new List<DialogParameter>() { new("DocumentId", document.Id) };

            Type documentInfoType = DocumentRepository.GetDocumentInfoType(document.Type);
            
            await DialogContentRepository.ShowDialogAsync(documentInfoType, dialogParameters, "Información del Documento");
        }

        private async Task AnnulDocumentAsync(UserDocumentDTO document)
        {
            var swalFireInfo = GetSwalFireInfo();

            bool updatedDocument = await SwalFireRepository.ShowLockSwalFireAsync(swalFireInfo);

            if (!updatedDocument) return;

            var annuledDocument = await ShowLoadingDialogAsync(document.Id);

            if (!annuledDocument) return;

            await SwalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo anular el documento de manera satisfactoria");

            await ChangePage(1);
        }

        private static SwalFireInfo GetSwalFireInfo()
        {
            string action = "anular";
            string actionTitle = "Anulación";

            string title = $"{actionTitle} del documento";
            string htmlContent = $"¿Está seguro que desea {action} el documento?";

            return new SwalFireInfo(title, htmlContent, SwalFireIcons.Warning, action);
        }

        private async Task<bool> ShowLoadingDialogAsync(string documentId)
        {
            string dialogTitle = $"Anulando el documento, por favor espere...";

            var functionToExecute = () => EvaluateDocumentAsync(documentId);

            return await DialogContentRepository.ShowLoadingDialogAsync(functionToExecute, dialogTitle);
        }

        private async Task<bool> EvaluateDocumentAsync(string documentId)
        {
            try
            {
                var documentEvaluationResponse = await HttpRepository.DeleteAsync($"api/documents/{documentId}");

                if (documentEvaluationResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo anular el documento");

                    return false;
                }

                return true;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo anular el documento");

                return false;
            }
           
        }

        private async Task ChangePage(int page)
        {
            currentPage = page - 1;

            paginatedUserDocuments = await GetDocumentsByUserAsync(SessionAccount.GetUser().Id);
        }

        private async Task SearchDocumentsAsync()
        {
            documentSearchLoading = true;

            await documentSearcherForm!.Validate();

            if (!documentSearcherForm.IsValid)
            {
                documentSearchLoading = false;

                return;
            }

            paginatedUserDocuments = await GetDocumentsByUserAsync(SessionAccount.GetUser().Id);

            documentSearchLoading = false;
        }

        private async Task<PaginatedUserDocumentDTO> GetDocumentsByUserAsync(string userId)
        {
            try
            {
                var userDocumentQueries = GetQueriesForUserDocuments();

                var solicitorDossiersResponse = await HttpRepository.GetAsync<PaginatedUserDocumentResponse>($"api/users/{userId}/documents{userDocumentQueries}");

                if (solicitorDossiersResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los documentos registrados");
                }

                var response = solicitorDossiersResponse.Response!;
                var documents = Mapper.Map<PaginatedUserDocumentDTO>(response);

                return documents;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los documentos registrados");
                return new(new List<UserDocumentDTO>(), 0);
            }
        }

        private string GetQueriesForUserDocuments()
        {

            string userRequestQueries = "?";
            userRequestQueries += $"page={System.Web.HttpUtility.UrlEncode(currentPage.ToString())}";
            userRequestQueries += $"&pagesize={System.Web.HttpUtility.UrlEncode(PageSize.ToString())}";

            var userDocumentFilters = UserDocumentRepository.ConvertToFilters(userDocumentFilter);

            if (userDocumentFilters.Count == 0) return userRequestQueries;

            userRequestQueries += "&" + userDocumentFilters.Select(filter => $"{filter.Key}={System.Web.HttpUtility.UrlEncode(filter.Value.ToString())}")
                .Aggregate((current, keys) => $"{current}&{keys}");

            return userRequestQueries;
        }


    }
}