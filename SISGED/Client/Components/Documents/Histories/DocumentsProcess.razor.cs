using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.DocumentProcess;
using System.Reflection.Metadata;

namespace SISGED.Client.Components.Documents.Histories
{
    public partial class DocumentsProcess
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

        private int TotalDocumentProcesses => (processesHistory.Count() + PageSize - 1) / PageSize;
        private IEnumerable<DocumentProcessInfo> processesHistory = new List<DocumentProcessInfo>();
        private IEnumerable<DocumentProcessInfo> paginatedProcessesHistory = new List<DocumentProcessInfo>();
        private bool pageLoading = true;

        protected override async Task OnInitializedAsync()
        {
            processesHistory = await GetProcessesByDocumentIdAsync(DocumentId);

            paginatedProcessesHistory = PaginateDocumentProcesses(0, PageSize);
                
            pageLoading = false;
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private IEnumerable<DocumentProcessInfo> PaginateDocumentProcesses(int page, int pageSize)
        {
            var paginatedProcessesHistory = processesHistory.Skip(page * pageSize).Take(pageSize);

            return paginatedProcessesHistory;
        }

        private void ChangePage(int page)
        {
            page--;

            paginatedProcessesHistory = PaginateDocumentProcesses(page, PageSize);
        }

        private async Task<IEnumerable<DocumentProcessInfo>> GetProcessesByDocumentIdAsync(string documentId)
        {
            try
            {
                var processesResponse = await HttpRepository.GetAsync<IEnumerable<DocumentProcessInfo>>($"api/documentProcesses/{documentId}");

                if (processesResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el historial de procesos del documento");
                }

                return processesResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el historial de procesos del documento");
                return new List<DocumentProcessInfo>();
            }
        }
    }
}