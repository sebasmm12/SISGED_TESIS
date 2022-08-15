using AutoMapper;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;
using SISGED.Shared.Models.Requests.User;
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

        }

        private string MapStepDocumentRequestUID(StepDocument stepDocument, StepGenericModel.StepDocument stepDocumentRequest)
        {
            return GenerateUID();
        }

        private string MapDocumentStepRequestUID(DocumentStep documentStep, StepGenericModel.DocumentStep documentStepRequest)
        {
            return GenerateUID();
        }


        private string GenerateUID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }

}
