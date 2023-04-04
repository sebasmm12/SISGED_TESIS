using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Contracts
{
    public interface ITemplateService : IGenericService
    {
        Task<Template> GetTemplateAsync(TemplateFilterDTO templateFilterDTO);
    }
}
