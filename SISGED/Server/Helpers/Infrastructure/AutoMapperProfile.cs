using AutoMapper;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.User;
using SISGED.Shared.Models.Responses.Role;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.User;

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
        }
    }
}
