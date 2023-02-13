using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.DocumentVersion;

namespace SISGED.Client.Components.Documents.Histories
{
    public partial class DocumentsVersion
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; } = default!;
        
        [Parameter]
        public string DocumentId { get; set; } = default!;
        [Parameter]
        public int PageSize { get; set; } = 5;

        private int TotalDocumentVersions => (contentVersions.Count() + PageSize - 1) / PageSize;
        private IEnumerable<DocumentVersionInfo> contentVersions = new List<DocumentVersionInfo>();
        private IEnumerable<DocumentVersionInfo> paginatedContentVersions = new List<DocumentVersionInfo>();
        private bool pageLoading = true;

        protected override async Task OnInitializedAsync()
        {
            contentVersions = await GetContentVersionsByDocumentIdAsync(DocumentId);

            paginatedContentVersions = PaginateDocumentVersions(0, PageSize);

            pageLoading = false;
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private IEnumerable<DocumentVersionInfo> PaginateDocumentVersions(int page, int pageSize)
        {
            var paginatedContentVersions = contentVersions.Skip(page * pageSize).Take(pageSize);

            return paginatedContentVersions;
        }

        private void ChangePage(int page)
        {
            page--;

            paginatedContentVersions = PaginateDocumentVersions(page, PageSize);
        }

        private async Task<IEnumerable<DocumentVersionInfo>> GetContentVersionsByDocumentIdAsync(string documentId)
        {
            try
            {
                var contentVersionsResponse = await HttpRepository.GetAsync<IEnumerable<DocumentVersionInfo>>($"api/documentVersions/{ documentId }");
                
                if(contentVersionsResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el historial de versiones del documento");
                }

                return contentVersionsResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el historial de versiones del documento");
                return new List<DocumentVersionInfo>();
            }
        }
    }
}