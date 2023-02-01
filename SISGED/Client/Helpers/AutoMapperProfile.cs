using AutoMapper;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Generics.Document;
using SISGED.Shared.Models.Requests.Account;
using SISGED.Shared.Models.Requests.User;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Dossier;
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
            CreateMap<Document, DocumentResponse>();

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
                .ForMember(disciplinaryOpenness => disciplinaryOpenness.SolicitorId, options => options.MapFrom(disciplinaryRegister => disciplinaryRegister.Solicitor.Id))
                .ForMember(disciplinaryOpenness => disciplinaryOpenness.ClientId, options => options.MapFrom(disciplinaryRegister => disciplinaryRegister.Client.ClientId))
                .ForMember(disciplinaryOpenness => disciplinaryOpenness.Participants, options => options.MapFrom(disciplinaryRegister => disciplinaryRegister.Participants.Select(participant => participant.Description).ToList()))
                .ForMember(disciplinaryOpenness => disciplinaryOpenness.ChargedDeeds, options => options.MapFrom(disciplinaryRegister => disciplinaryRegister.ChargedDeeds.Select(participant => participant.Description).ToList()));


            // Solicitor Dossier Request Mapper
            CreateMap<SolicitorDossierRequestRegisterDTO, SolicitorDossierRequestResponseContent>()
                .ForMember(solicitorRequest => solicitorRequest.SolicitorId, options => options.MapFrom(solicitorRequestRegister => solicitorRequestRegister.Solicitor.Id))
                .ForMember(solicitorRequest => solicitorRequest.ClientId, options => options.MapFrom(solicitorRequestRegister => solicitorRequestRegister.Client.ClientId));

            // Dictum Mapper
            CreateMap<DictumRegisterDTO, DictumResponseContent>()
                .ForMember(dictumContent => dictumContent.SolicitorId, options => options.MapFrom(dictumRegister => dictumRegister.Solicitor.Id))
                .ForMember(dictumContent => dictumContent.ComplaintId, options => options.MapFrom(dictumRegister => dictumRegister.Client.ClientId))
                .ForMember(dictumContent => dictumContent.Observations, options => options.MapFrom(dictumRegister => dictumRegister.Observations.Select(observation => observation.Description).ToList()))
                .ForMember(dictumContent => dictumContent.Recomendations, options => options.MapFrom(dictumRegister => dictumRegister.Recommendations.Select(recommendation => recommendation.Description).ToList()));

            // Resolution Request Mapper
            CreateMap<ResolutionRegisterDTO, ResolutionResponseContent>()
                .ForMember(resolutionContent => resolutionContent.Participants, options => options.MapFrom(resolutionRegister => resolutionRegister.Participants.Select(participant => participant.Description).ToList()))
                .ForMember(resolutionContent => resolutionContent.Penalty, options => options.MapFrom(resolutionRegister => resolutionRegister.Penalty.Id))
                .ForMember(resolutionContent => resolutionContent.SolicitorId, options => options.MapFrom(resolutionRegister => resolutionRegister.Solicitor.Id))
                .ForMember(resolutionContent => resolutionContent.ClientId, options => options.MapFrom(resolutionRegister => resolutionRegister.Client.ClientId));

            // Solicitor Dossier Shipment
            CreateMap<SolicitorDossierShipmentRegisterDTO, SolicitorDossierShipmentResponseContent>()
                .ForMember(solicitorDossierShipmentContent => solicitorDossierShipmentContent.SolicitorId, options => options.MapFrom(solicitorDossierShipmentRegister => solicitorDossierShipmentRegister.Solicitor.Id));


            CreateMap<DossierTrayResponse, Item>()
                .ForMember(item => item.Name, options => options.MapFrom(dossierTray => dossierTray.Type))
                .ForMember(item => item.ItemStatus, options => options.MapFrom(dossierTray => dossierTray.Document!.State))
                .ForMember(item => item.OriginPlace, options => options.MapFrom(_ => "outputs"))
                .ForMember(item => item.Value, options => options.MapFrom(dossierTray => dossierTray))
                .ForMember(item => item.Description, options => options.MapFrom(dossierTray => dossierTray.Document!.Type));


            // Document Derivation
            CreateMap<DocumentInfo, DocumentResponse>()
                .ForMember(documentResponse => documentResponse.UrlAnnex, options => options.MapFrom(documentInfo => documentInfo.AttachedUrls));

            // Login
            CreateMap<UserLoginDTO, AccountLoginRequest>();

            //User Self Register
            CreateMap<UserSelfRegisterDTO, UserRegisterRequest>()

                .ForMember(userRequest => userRequest.UserName, options => options.MapFrom(userSelfRegister => userSelfRegister.Username))
                .ForMember(userRequest => userRequest.Data, options => options.MapFrom(userSelfRegister => new UserData
                {
                    Name = userSelfRegister.Name,
                    LastName = userSelfRegister.LastName,
                    DocumentNumber = userSelfRegister.DocumentNumber,
                    DocumentType = userSelfRegister.DocumentType.Name,
                    Address = userSelfRegister.Address,
                    Email = userSelfRegister.Email,
                    BornDate = userSelfRegister.BornDate.GetValueOrDefault()
                }))
                .ForMember(userRequest => userRequest.Password, options => options.MapFrom(userSelfRegister => userSelfRegister.Password));
        }
    }
}
