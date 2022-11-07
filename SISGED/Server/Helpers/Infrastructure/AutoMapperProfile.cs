using AutoMapper;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;
using SISGED.Shared.Models.Requests.User;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.Appeal;
using SISGED.Shared.Models.Responses.Document.BPNRequest;
using SISGED.Shared.Models.Responses.Document.Dictum;
using SISGED.Shared.Models.Responses.Document.InitialRequest;
using SISGED.Shared.Models.Responses.Document.Resolution;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Role;
using SISGED.Shared.Models.Responses.Solicitor;
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
            CreateMap<Solicitor, SolicitorInfoResponse>().ReverseMap();

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

            // Resolution Mapper
            CreateMap<ResolutionContent, ResolutionContentInfo>();
            CreateMap<Resolution, ResolutionInfoResponse>();

            // Appeal Mapper
            CreateMap<AppealContent, AppealContentInfo>();
            CreateMap<Appeal, AppealInfoResponse>();

            // Initial Request Mapper
            CreateMap<InitialRequestContent, InitialRequestContentInfo>();
            CreateMap<InitialRequest, InitialRequestInfoResponse>();

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
