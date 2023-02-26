using SISGED.Shared.Models.Responses.DocumentEvaluation;

namespace SISGED.Server.Services.Contracts
{
    public interface IDocumentEvaluationService : IGenericService
    {
        Task<IEnumerable<DocumentEvaluationInfo>> GetEvaluationsByDocumentIdAsync(string documentId);
    }
}
