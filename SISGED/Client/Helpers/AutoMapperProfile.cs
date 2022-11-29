using AutoMapper;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Document;

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
        }
    }
}
