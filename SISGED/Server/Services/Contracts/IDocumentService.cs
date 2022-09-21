using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.BPNDocument;
using SISGED.Shared.Models.Responses.Document.BPNResult;
using SISGED.Shared.Models.Responses.Document.DisciplinaryOpenness;
using SISGED.Shared.Models.Responses.Document.SignConclusion;
using SISGED.Shared.Models.Responses.Document.SolicitorDesignationDocument;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierRequest;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Contracts
{
    public interface IDocumentService : IGenericService
    {
        Task<Appeal> GetAppealDocumentAsync(string documentId);
        Task<BPNDocumentInfoResponse> GetBPNDocumentAsync(string documentId);
        Task<BPNRequest> GetBPNRequestDocumentAsync(string documentId);
        Task<BPNResultInfoResponse> GetBPNResultAsync(string documentId);
        Task<Dictum> GetDictumDocumentAsync(string documentId);
        Task<DisciplinaryOpennessInfoResponse> GetDisciplinaryOpennessAsync(string documentId);
        Task<DocumentResponse> GetDocumentAsync(string documentId);
        Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAndAreaAsync(DocumentsByMonthAndAreaQuery documentsByMonthAndAreaQuery);
        Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery);
        Task<IEnumerable<DocumentByStateResponse>> GetDocumentsByStateAsync(DocumentsByStateQuery documentsByStateQuery);
        Task<IEnumerable<ExpiredDocumentsResponse>> GetExpiredDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery);
        Task<InitialRequest> GetInitialRequestDocumentAsync(string documentId);
        Task<Resolution> GetResolutionDocumentAsync(string documentId);
        Task<SignConclusionInfoResponse> GetSignConclusionAsync(string documentId);
        Task<SolicitorDesignationInfoResponse> GetSolicitorDesignationAsync(string documentId);
        Task<SolicitorDossierRequestInfoResponse> GetSolicitorDossierRequestAsync(string documentId);
        Task<SolicitorDossierShipmentInfoResponse> GetSolicitorDossierShipmentAsync(string documentId);
        Task<IEnumerable<UserRequestDocumentResponse>> GetUserRequestDocumentsAsync(string documentNumber);
        Task<IEnumerable<UserRequestWithPublicDeedResponse>> GetUserRequestsWithPublicDeedAsync(string documentNumber);
        Task UpdateDocumentProcessAsync(Process proccess, string documentId);
    }
}
