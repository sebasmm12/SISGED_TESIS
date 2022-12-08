using AutoMapper;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Client.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User Request Mapper
            CreateMap<UserRequestRegisterDTO, InitialRequestResponseContent>()
                .ForMember(initialRequest => initialRequest.SolicitorId, options => options.MapFrom(userRequest => userRequest.Solicitor.Id))
                .ForMember(initialRequest => initialRequest.RequestTypeId, options => options.MapFrom(userRequest => userRequest.DocumentType.Id));

            // Complaint Request Mapper
            CreateMap<ComplaintRequestRegisterDTO, ComplaintRequestResponseContent>()
                .ForMember(complaintRequest => complaintRequest.SolicitorId, options => options.MapFrom(complaintRequestRegister => complaintRequestRegister.Solicitor.Id))
                .ForMember(complaintRequest => complaintRequest.ClientId, options => options.MapFrom(complaintRequestRegister => complaintRequestRegister.Client.ClientId))
                .ForMember(complaintRequest => complaintRequest.ComplaintType, options => options.MapFrom(complaintRequestRegister => complaintRequestRegister.ComplaintType.Id))
                .ForMember(complaintRequest => complaintRequest.DeliveryDate, options => options.MapFrom(_ => DateTime.UtcNow.AddHours(-5)));

            // Disciplinary Openness Mapper
            CreateMap<DisciplinaryOpennessRegisterDTO, DisciplinaryOpennessResponseContent>()
                .ForMember(complaintRequest => complaintRequest.SolicitorId, options => options.MapFrom(complaintRequestRegister => complaintRequestRegister.Solicitor.Id));
            
            // Solicitor Dossier Request Mapper
            CreateMap<SolicitorDossierRequestRegisterDTO, SolicitorDossierRequestResponseContent>()
                .ForMember(complaintRequest => complaintRequest.SolicitorId, options => options.MapFrom(complaintRequestRegister => complaintRequestRegister.Solicitor.Id));
        }
    }
}
