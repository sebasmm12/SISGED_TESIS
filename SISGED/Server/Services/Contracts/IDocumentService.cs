using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Contracts
{
    public interface IDocumentService : IGenericService
    {
        Task<Appeal> GetAppealDocumentAsync(string documentId);
        Task<BPNRequest> GetBPNRequestDocumentAsync(string documentId);
        Task<Dictum> GetDictumDocumentAsync(string documentId);
        Task<DocumentResponse> GetDocumentAsync(string documentId);
        Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAndAreaAsync(DocumentsByMonthAndAreaQuery documentsByMonthAndAreaQuery);
        Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery);
        Task<IEnumerable<DocumentByStateResponse>> GetDocumentsByStateAsync(DocumentsByStateQuery documentsByStateQuery);
        Task<IEnumerable<ExpiredDocumentsResponse>> GetExpiredDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery);
        Task<InitialRequest> GetInitialRequestDocumentAsync(string documentId);
        Task<Resolution> GetResolutionDocumentAsync(string documentId);
        Task<IEnumerable<UserRequestDocumentResponse>> GetUserRequestDocumentsAsync(string documentNumber);
        Task<IEnumerable<UserRequestWithPublicDeedResponse>> GetUserRequestsWithPublicDeedAsync(string documentNumber);
        Task UpdateDocumentProcessAsync(Process proccess, string documentId);
    }
}
