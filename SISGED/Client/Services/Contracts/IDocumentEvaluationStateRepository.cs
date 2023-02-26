using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IDocumentEvaluationStateRepository
    {
        DocumentState GetDocumentEvaluationState(bool isApproved);
    }
}
