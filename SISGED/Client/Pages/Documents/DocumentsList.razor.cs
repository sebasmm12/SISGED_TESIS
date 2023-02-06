using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.UserDocument;
using SISGED.Shared.Validators;
using System.Text.Json;

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
        public IDialogService DialogService { get; set; } = default!;
        [Inject]
        public IFilterRepository<UserDocumentFilterDTO> UserDocumentRepository { get; set; } = default!;
        [Inject]
        public IDossierRepository DossierRepository { get; set; } = default!;
        [Inject]
        public IDocumentStateRepository DocumentStateRepository { get; set; } = default!;
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

                await JSRuntime.InvokeVoidAsync("console.log", documents);
                
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