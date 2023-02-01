using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Queries.UserDocument;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.UserDocument;

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
        public IFilterRepository<UserDocumentFilter> UserDocumentRepository { get; set; } = default!;

        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        [Parameter]
        public int PageSize { get; set; } = 5;

        private bool documentsLoading = true;
        private PaginatedUserDocumentResponse paginatedUserDocuments = default!;
        private UserDocumentFilter userDocumentFilter = new();
        private int currentPage = 0;

        protected override async Task OnInitializedAsync()
        {
            paginatedUserDocuments = await GetDocumentsByUserAsync(SessionAccount.GetUser().Id);

            documentsLoading = false;
        }

        private async Task<PaginatedUserDocumentResponse> GetDocumentsByUserAsync(string userId)
        {
            try
            {
                var userDocumentQueries = GetQueriesForUserDocuments();

                var solicitorDossiersResponse = await HttpRepository.GetAsync<PaginatedUserDocumentResponse>($"api/solicitorsDossiers/{userId}{userDocumentQueries}");

                if (solicitorDossiersResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los documentos registrados");
                }

                return solicitorDossiersResponse.Response!;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los documentos registrados");
                return new(new List<UserDocumentResponse>(), 0);
            }
        }

        private string GetQueriesForUserDocuments()
        {

            string userRequestQueries = "?";
            userRequestQueries += $"page={System.Web.HttpUtility.UrlEncode(currentPage.ToString())}";
            userRequestQueries += $"&pagesize={System.Web.HttpUtility.UrlEncode(PageSize.ToString())}";

            var userDocumentFilters = UserDocumentRepository.ConvertToFilters(userDocumentFilter);

            if (userDocumentFilters.Count > 0) return userRequestQueries;
            
            userRequestQueries += "&" + userDocumentFilters.Select(filter => $"{filter.Key}={System.Web.HttpUtility.UrlEncode(filter.Value.ToString())}")
                .Aggregate((current, keys) => $"{current}&{keys}");

            return userRequestQueries;
        }


    }
}