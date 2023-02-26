using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Components.Documents.Histories
{
    public partial class DocumentInfo
    {
        [Inject]
        public IDocumentStateRepository DocumentStateRepository { get; set; } = default!;
    
        [Parameter]
        public UserDocumentDTO Document { get; set; } = default!;
        [Parameter]
        public EventCallback<UserDocumentDTO> DocumentAnnulment { get; set; }
        [Parameter]
        public EventCallback<UserDocumentDTO> DocumentVersionHistoryInfo { get; set; }
        [Parameter]
        public EventCallback<UserDocumentDTO> DocumentProcessHistoryInfo { get; set; }
        [Parameter]
        public EventCallback<UserDocumentDTO> DocumentVisualization { get; set; }
        [Parameter]
        public EventCallback<UserDocumentDTO> DocumentEvaluation { get; set; }

        private IEnumerable<string> annulmentInValidStates = new List<string>() { "evaluado", "anulado" };

        private Color GetDocumentStateColor(string documentState)
        {
            return DocumentStateRepository.GetDocumentState(documentState).Color;
        }

        private async Task AnnulDocumentAsync()
        {
            await DocumentAnnulment.InvokeAsync(Document);
        }

        private async Task ShowDocumentVersionHistoryAsync()
        {
            await DocumentVersionHistoryInfo.InvokeAsync(Document);
        }

        private async Task ShowDocumentProcessHistoryAsync()
        {
            await DocumentProcessHistoryInfo.InvokeAsync(Document);
        }

        private async Task ShowDocumentInfoAsync()
        {
            await DocumentVisualization.InvokeAsync(Document);
        }

        private async Task ShowDocumentEvaluationAsync()
        {
            await DocumentEvaluation.InvokeAsync(Document);
        }

    }
}