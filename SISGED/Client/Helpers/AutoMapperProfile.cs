using AutoMapper;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Client.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Document Request Mapper
            CreateMap<ComplaintRequest, DocumentResponse>();

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

            CreateMap<DossierTrayResponse, Item>()
                .ForMember(item => item.Name, options => options.MapFrom(dossierTray => dossierTray.Type))
                .ForMember(item => item.ItemStatus, options => options.MapFrom(dossierTray => dossierTray.Document!.State))
                .ForMember(item => item.OriginPlace, options => options.MapFrom(_ => "outputs"))
                .ForMember(item => item.Value, options => options.MapFrom(dossierTray => dossierTray))
                .ForMember(item => item.Description, options => options.MapFrom(dossierTray => dossierTray.Document!.Type));
        }
    }
}
