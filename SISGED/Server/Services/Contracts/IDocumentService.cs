﻿using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Document;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Queries.UserDocument;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.BPNDocument;
using SISGED.Shared.Models.Responses.Document.BPNResult;
using SISGED.Shared.Models.Responses.Document.ComplaintRequest;
using SISGED.Shared.Models.Responses.Document.Dictum;
using SISGED.Shared.Models.Responses.Document.DisciplinaryOpenness;
using SISGED.Shared.Models.Responses.Document.Resolution;
using SISGED.Shared.Models.Responses.Document.SignConclusion;
using SISGED.Shared.Models.Responses.Document.SolicitorDesignationDocument;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierRequest;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Statistic;
using SISGED.Shared.Models.Responses.UserDocument;
using SISGED.Shared.Models.Responses.Document.SessionResolutions;

namespace SISGED.Server.Services.Contracts
{
    public interface IDocumentService : IGenericService
    {
        Task<Appeal> GetAppealDocumentAsync(string documentId);
        Task<BPNDocumentInfoResponse> GetBPNDocumentAsync(string documentId);
        Task<BPNRequest> GetBPNRequestDocumentAsync(string documentId);
        Task<BPNResultInfoResponse> GetBPNResultAsync(string documentId);
        Task<DictumInfoResponse> GetDictumDocumentAsync(string documentId);
        Task<DisciplinaryOpennessInfoResponse> GetDisciplinaryOpennessAsync(string documentId);
        Task<DocumentResponse> GetDocumentAsync(string documentId);
        Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAndAreaAsync(DocumentsByMonthAndAreaQuery documentsByMonthAndAreaQuery);
        Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery);
        Task<IEnumerable<DocumentByStateResponse>> GetDocumentsByStateAsync(DocumentsByStateQuery documentsByStateQuery);
        Task<IEnumerable<ExpiredDocumentsResponse>> GetExpiredDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery);
        Task<InitialRequest> GetInitialRequestDocumentAsync(string documentId);
        Task<ResolutionInfoResponse> GetResolutionDocumentAsync(string documentId);
        Task<SignConclusionInfoResponse> GetSignConclusionAsync(string documentId);
        Task<SolicitorDesignationInfoResponse> GetSolicitorDesignationAsync(string documentId);
        Task<SolicitorDossierRequestInfoResponse> GetSolicitorDossierRequestAsync(string documentId);
        Task<SolicitorDossierShipmentInfoResponse> GetSolicitorDossierShipmentAsync(string documentId);
        Task<IEnumerable<UserRequestDocumentResponse>> GetUserRequestDocumentsAsync(string documentNumber);
        Task<IEnumerable<UserRequestResponse>> GetUserRequestsWithPublicDeedAsync(UserRequestPaginationQuery userRequestPaginationQuery);
        Task UpdateDocumentProcessAsync(Process proccess, string documentId);
        Task<SolicitorDesignationDocument> SolicitorDesignationOfficeRegisterAsync(DossierWrapper dossier, List<string> url2);
        Task<BPNDocument> RegisterBPNOfficeAsync(DossierWrapper dossierWrapper, List<string> url2, string url);
        Task<BPNRequest> RegisterBPNRquestAsync(DossierWrapper dossierWrapper, List<string> url2);
        Task<SignExpeditionRequest> RegisterSignExpeditionRequestAsync(DossierWrapper expedientewrapper, List<string> url2, string urlData);
        Task<InitialRequest> InitialRequestRegisterAsync(InitialRequest documentIR);
        Task<SignConclusion> singConclusionERegisterAsync(DossierWrapper dossierWrapper, List<string> url2, string documentId);
        Task<DisciplinaryOpenness> DisciplinaryOpennessRegisterAsync(DisciplinaryOpenness disciplinaryOpenness);
        Task<SolicitorDossierRequest> SolicitorDossierRequestRegisterAsync(SolicitorDossierRequest solicitorDossierRequest);
        Task<Dictum> RegisterDictumAsync(Dictum dictum);
        Task<Resolution> ResolutionRegisterAsync(Resolution resolution);
        Task<SessionResolution> RegisterSessionResolutionAsync(SessionResolution sessionResolution);
        Task<BPNResult> BPNResultRegisterAsync(BPNResultResponse DTO, List<string> url2, string UserId, string dossierId, string inputDocId, string documentRequestId);
        Task<SolicitorDossierShipment> RegisterSolicitorDossierShipmentAsync(SolicitorDossierShipment solicitorDossierShipment);
        Task<Document> EvaluateDocumentAsync(DocumentEvaluationRequest documentEvaluationRequest, User user);
        Task<Document> GenerateDocumentAsync(DocumentGenerationDTO document);
        Task<Document> ModifyStateDocumentAsync(DocumentRequest document);
        Task<SolicitorDesignationDocument> UpdateDocumentODNAsync(DossierWrapper dossierWrapper, List<string> url2);
        Task<Appeal> AppealDocumentUpdateAsync(DossierWrapper dossierWrapper, string urlData, List<string> url2);
        Task<DisciplinaryOpenness> DisciplinaryOpennessDocumentUpdateAsync(DossierWrapper dossierWrapper, List<string> url2);
        Task<SignConclusion> UpdateSignConclusionDocumentAsync(DossierWrapper dossierWrapper, List<string> url2);
        Task<Dictum> UpdateDictumDocumentAsync(DossierWrapper dossierWrapper, List<string> url2);
        Task<BPNDocument> UpdateBPNOfficeDocumentAsync(DossierWrapper dossierWrapper, List<string> url2);
        Task<Resolution> UpdateResolutionDocumentAsync(DossierWrapper dossierWrapper, string urlData, List<string> url2);
        Task UpdateSENDocumentAsync(DossierWrapper dossierWrapper);
        Task<BPNResult> UpdateBPNResultDocumentAsync(DossierWrapper dossierWrapper, List<string> url2);
        Task UpdateInitialRequestDocumentAsync(DossierWrapper dossierWrapper);
        Task UpdateEENDocumentAsync(DossierWrapper dossierWrapper);
        Task UpdateInitialRequestStateAsync(DossierWrapper dossierWrapper);
        Task<long> CountUserRequestAsync(string clientId);
        Task<ComplaintRequest> RegisterComplaintRequestAsync(ComplaintRequest complaintRequest);
        Task<IEnumerable<UserDocumentResponse>> GetDocumentsByUserAsync(string userId, UserDocumentPaginationQuery userDocumentPaginationQuery);
        Task<int> CountDocumentsByUserAsync(string userId, UserDocumentPaginationQuery userDocumentPaginationQuery);
        Task<ComplaintRequestInfoResponse> GetComplaintRequestDocumentAsync(string documentId);
        Task<bool> VerifyDocumentAnnulmentAsync(string documentId);
        Task<Document> AnnulDocumentAsync(string documentId, User user);
        Task<SessionResolutionInfoResponse> GetSessionResolutionDocumentAsync(string documentId);
        Task UpdateDocumentDueDateAsync(RegisteredDocumentDTO registeredDocument);
        Task UpdateDocumentEndDateAsync(string documentId, DateTime endDate);
    }
}
