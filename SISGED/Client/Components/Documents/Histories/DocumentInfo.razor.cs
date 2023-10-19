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
        public int _md { get; set; } = 6;
        public int _lg { get; set; } = 4;
        public bool _annulmentActivation { get; set; } = true;
        [Parameter]
        public int? MdParam { get; set; }
        [Parameter]
        public int? LgParam { get; set; }
        [Parameter]
        public bool? AnnulmentActivation { get; set; }
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

        protected override void OnParametersSet()
        {
            if (MdParam.HasValue)
            {
                _md = MdParam.Value;
            }
            if (LgParam.HasValue)
            {
                _lg = LgParam.Value;
            }
        }

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