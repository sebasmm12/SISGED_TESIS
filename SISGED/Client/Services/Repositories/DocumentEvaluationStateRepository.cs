using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DocumentEvaluationStateRepository : IDocumentEvaluationStateRepository
    {
        private readonly IDictionary<bool, DocumentState> _documentEvaluationStateColors = new Dictionary<bool, DocumentState>()
        {
            { true, new(Color.Success, "Aprobado")  },
            { false, new(Color.Error, "Rechazado") }
        };

        public DocumentState GetDocumentEvaluationState(bool isApproved)
        {
            return _documentEvaluationStateColors.First(documentEvaluationStateColor => 
                            documentEvaluationStateColor.Key == isApproved).Value;
        }
    }
}
