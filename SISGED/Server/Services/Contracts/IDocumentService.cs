﻿using Microsoft.AspNetCore.Mvc;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Contracts
{
    public interface IDocumentService : IGenericService
    {
        Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAndAreaAsync(DocumentsByMonthAndAreaQuery documentsByMonthAndAreaQuery);
        Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery);
        Task<IEnumerable<DocumentByStateResponse>> GetDocumentsByStateAsync(DocumentsByStateQuery documentsByStateQuery);
        Task<IEnumerable<ExpiredDocumentsResponse>> GetExpiredDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery);
        Task UpdateDocumentProcessAsync(Process proccess, string documentId);
        Task<SolicitorDesignationDocument> SolicitorDesignationOfficeRegisterAsync(SolicitorDesignationDocumentRegister dossier, List<string> url2);
        Task<BPNDocument> RegisterBPNOfficeAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2, string url);
        Task<BPNRequest> RegisterBPNRquestAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2);
        Task<ComplaintRequest> RegisterComplaintRequestAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2, string urlData);
        Task<SignExpeditionRequest> RegisterSignExpeditionRequestAsync(SolicitorDesignationDocumentRegister expedientewrapper, List<string> url2, string urlData);
        Task<InitialRequest> InitialRequestRegisterAsync(InitialRequest documentIR);
        Task<SignConclusion> singConclusionERegisterAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2, string documentId);
        Task<DisciplinaryOpenness> DisciplinaryOpennessRegisterAsync(DisciplinaryOpennessResponse DTO, string urlData, List<string> url2, string userId, string dossierID, string inputDocId);
        Task<Dictum> DictumRegisterAsync(DictumResponse dTO, SolicitorDesignationDocumentRegister expedientewrapper, List<string> url2);
        Task<Resolution> ResolutionRegisterAsync(ResolutionResponse DTO, string urldata, List<string> url2, string idusuario, string idexpediente, string iddocentrada, string iddocumentoSolicitud);
        Task<BPNResult> BPNResultRegisterAsync(BPNResultResponse DTO, List<string> url2, string UserId, string dossierId, string inputDocId, string documentRequestId);
        Task<SolicitorDossierShipment> SolicitorDossierShipmentRegisterAsync(SolicitorDossierShipmentResponse DTO, SolicitorDesignationDocumentRegister expedientewrapper, List<string> url2);
        Task<Document> ModifyStateAsync(Evaluation document, string docId);
        Task<Document> GenerateDocumentAsync(GenerateDocumentRequest document);
        Task<Document> ModifyStateDocumentAsync(DocumentRequest document);
        Task<SolicitorDesignationDocument> UpdateDocumentODNAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2);
        Task<Appeal> AppealDocumentUpdateAsync(SolicitorDesignationDocumentRegister dossierWrapper, string urlData, List<string> url2);
        Task<DisciplinaryOpenness> DisciplinaryOpennessDocumentUpdateAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2);
        Task<SignConclusion> UpdateSignConclusionDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2);
        Task<Dictum> UpdateDictumDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2);
        Task<BPNDocument> UpdateBPNOfficeDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2);
        Task<Resolution> UpdateResolutionDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, string urlData, List<string> url2);
        Task UpdateSENDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper);
        Task<BPNResult> UpdateBPNResultDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2);
        Task UpdateInitialRequestDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper);

    }
}
