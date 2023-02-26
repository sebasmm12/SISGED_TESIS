using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Services.Contracts;
using DocumentEvaluationResponse = SISGED.Shared.Models.Responses.DocumentEvaluation;

namespace SISGED.Client.Components.Documents.Histories
{
    public partial class DocumentsEvaluation
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

        private int TotalDocumentEvaluations => (evaluations.Count() + PageSize - 1) / PageSize;
        private IEnumerable<DocumentEvaluationResponse.DocumentEvaluationInfo> evaluations = new List<DocumentEvaluationResponse.DocumentEvaluationInfo>();
        private IEnumerable<DocumentEvaluationResponse.DocumentEvaluationInfo> paginatedEvaluations = new List<DocumentEvaluationResponse.DocumentEvaluationInfo>();
        private bool pageLoading = true;

        protected override async Task OnInitializedAsync()
        {
            evaluations = await GetProcessesByDocumentIdAsync(DocumentId);

            paginatedEvaluations = PaginateDocumentProcesses(0, PageSize);

            pageLoading = false;
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private IEnumerable<DocumentEvaluationResponse.DocumentEvaluationInfo> PaginateDocumentProcesses(int page, int pageSize)
        {
            var paginatedEvaluations = evaluations.Skip(page * pageSize).Take(pageSize);

            return paginatedEvaluations;
        }

        private void ChangePage(int page)
        {
            page--;

            paginatedEvaluations = PaginateDocumentProcesses(page, PageSize);
        }

        private async Task<IEnumerable<DocumentEvaluationResponse.DocumentEvaluationInfo>> GetProcessesByDocumentIdAsync(string documentId)
        {
            try
            {
                var evaluationsResponse = await HttpRepository.GetAsync<IEnumerable<DocumentEvaluationResponse.DocumentEvaluationInfo>>($"api/documentEvaluations/{documentId}");

                if (evaluationsResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el historial de procesos del documento");
                }

                return evaluationsResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener las evaluaciones del documento");
                return new List<DocumentEvaluationResponse.DocumentEvaluationInfo>();
            }
        }
    }
}