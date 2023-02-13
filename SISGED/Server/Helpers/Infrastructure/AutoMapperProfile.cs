using AutoMapper;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Requests.Step;
using SISGED.Shared.Models.Requests.User;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.Appeal;
using SISGED.Shared.Models.Responses.Document.BPNRequest;
using SISGED.Shared.Models.Responses.Document.Dictum;
using SISGED.Shared.Models.Responses.Document.InitialRequest;
using SISGED.Shared.Models.Responses.Document.Resolution;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.DocumentVersion;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Role;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.SolicitorDossier;
using SISGED.Shared.Models.Responses.Step;
using SISGED.Shared.Models.Responses.User;
using StepGenericModel = SISGED.Shared.Models.Generics.Step;

namespace SISGED.Server.Helpers.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User Mapper
            CreateMap<User, UserInfoResponse>().ReverseMap();
            CreateMap<UserUpdateRequest, User>().ReverseMap();

            // Rol Mapper
            CreateMap<Role, RoleInfoResponse>().ReverseMap();

            // Solicitor Mapper
            CreateMap<Solicitor, AutocompletedSolicitorResponse>()
                .ForMember(AutocompletedSolicitorResponse => AutocompletedSolicitorResponse.SolicitorOfficeName, options => options.MapFrom(solicitor => solicitor.SolicitorOffice.Name));

            
            CreateMap<AutocompletedSolicitorResponse, Solicitor>();

            // Steps Mapper and its related classes
            CreateMap<DocumentStep, StepGenericModel.DocumentStep>()
                .ForMember(documentStepRequest => documentStepRequest.Uid, options => options.MapFrom(MapDocumentStepRequestUID));

            CreateMap<StepGenericModel.DocumentStep, DocumentStep>();

            CreateMap<StepDocument, StepGenericModel.StepDocument>()
                .ForMember(stepDocumentRequest => stepDocumentRequest.Uid, options => options.MapFrom(MapStepDocumentRequestUID));

            CreateMap<StepGenericModel.StepDocument, StepDocument>();

            CreateMap<Step, DossierStepsResponse>()
                .ForMember(stepRequest => stepRequest.Documents, options => new List<StepRegisterRequest>());

            CreateMap<Step, StepRegisterRequest>()
                .ForMember(stepRequest => stepRequest.Documents, options => new List<StepRegisterRequest>());

            CreateMap<StepRegisterRequest, Step>();

            // Dossier Mapper
            CreateMap<Dossier, DossierInfoResponse>();

            // Documents Mapper and its related classes
            CreateMap<Document, DocumentInfoResponse>();

            // BPN Request Mapper
            CreateMap<BPNRequestContent, BPNRequestContentInfo>();
            CreateMap<BPNRequest, BPNRequestInfoResponse>();

            // Dictum Mapper
            CreateMap<DictumContent, DictumContentInfo>();
            CreateMap<Dictum, DictumInfoResponse>();
            CreateMap<DictumResponseContent, DictumContent>();

            // Resolution Mapper
            CreateMap<ResolutionContent, ResolutionContentInfo>();
            CreateMap<Resolution, ResolutionInfoResponse>();

            // Appeal Mapper
            CreateMap<AppealContent, AppealContentInfo>();
            CreateMap<Appeal, AppealInfoResponse>();

            // Initial Request Mapper
            CreateMap<InitialRequestContent, InitialRequestContentInfo>();
            CreateMap<InitialRequest, InitialRequestInfoResponse>();
            CreateMap<InitialRequestResponseContent, InitialRequestContent>();

            // Document Type Mapper
            CreateMap<DocumentType, DocumentTypeInfoResponse>().ReverseMap();

            // Complaint Type Mapper
            CreateMap<ComplaintRequestResponseContent, ComplaintRequestContent>();

            // Disciplinary Openness Type Mapper
            CreateMap<DisciplinaryOpennessResponse, DisciplinaryOpenness>();
            CreateMap<DisciplinaryOpennessResponseContent, DisciplinaryOpennessContent>()
                .ForMember(disciplinaryContent => disciplinaryContent.ComplainantName, options => options.MapFrom(disciplinaryResponse => disciplinaryResponse.Complainant))
                .ForMember(disciplinaryContent => disciplinaryContent.AudiencePlace, options => options.MapFrom(disciplinaryResponse => disciplinaryResponse.AudienceLocation))
                .ForMember(disciplinaryContent => disciplinaryContent.ImputedFacts, options => options.MapFrom(disciplinaryResponse => disciplinaryResponse.ChargedDeeds))
                .ForMember(disciplinaryContent => disciplinaryContent.Url, options => options.MapFrom(disciplinaryResponse => disciplinaryResponse.URL));
            
            // Solicitor Dossier Request Type Mapper
            CreateMap<SolicitorDossierRequestResponse, SolicitorDossierRequest>();
            CreateMap<SolicitorDossierRequestResponseContent, SolicitorDossierRequestContent>()
                .ForMember(solicitorContent => solicitorContent.IssueDate, options => options.MapFrom(solicitorResponse => solicitorResponse.DateIssue));
            CreateMap<SolicitorDossier, SolicitorDossierByIdsResponse>();

            // Solicitor Dossier Shipment Mapper
            CreateMap<SolicitorDossierShipmentResponseContent, SolicitorDossierShipmentContent>();

            // Resolution Type Mapper
            CreateMap<ResolutionResponse, Resolution>();
            CreateMap<ResolutionResponseContent, ResolutionContent>()
                .ForMember(resolutionContent => resolutionContent.Sanction, options => options.MapFrom(resolutionResponse => resolutionResponse.Penalty));

            // Solicitor Dossier Mapper
            CreateMap<SolicitorDossier, SolicitorDossierResponse>();

            // Accounts Mapper
            CreateMap<UserRegisterRequest, User>();

            // Document Generation
            CreateMap<GenerateDocumentRequest, DocumentGenerationDTO>()
                .ForMember(documentGeneration => documentGeneration.Sign, options => options.MapFrom(_ => string.Empty))
                .ForMember(documentGeneration => documentGeneration.GeneratedURL, options => options.MapFrom(_ => string.Empty));

            // Document Version Mapper
            CreateMap<ContentVersion, DocumentVersionInfo>();
        }

        private string MapStepDocumentRequestUID(StepDocument stepDocument, StepGenericModel.StepDocument stepDocumentRequest)
        {
            return GenerateUID();
        }

        private string MapDocumentStepRequestUID(DocumentStep documentStep, StepGenericModel.DocumentStep documentStepRequest)
        {
            return GenerateUID();
        }


        private static string GenerateUID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }

}
