using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using DocumentEvaluationResponse = SISGED.Shared.Models.Responses.DocumentEvaluation;

namespace SISGED.Client.Components.Documents.Histories
{
    public partial class DocumentEvaluationInfo
    {
        [Inject]
        public IDocumentEvaluationStateRepository DocumentEvaluationStateRepository { get; set; } = default!;

        [Parameter]
        public DocumentEvaluationResponse.DocumentEvaluationInfo Evaluation { get; set; } = default!;

        private DocumentState evaluationState = default!;

        protected override void OnInitialized()
        {
            evaluationState = GetDocumentEvaluationState(Evaluation.IsApproved);
        }

        private DocumentState GetDocumentEvaluationState(bool isApproved)
        {
            return DocumentEvaluationStateRepository.GetDocumentEvaluationState(isApproved);
        }
    }
}