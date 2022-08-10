using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Contracts
{
    public interface ISolicitorService: IGenericService
    {
        Task<Solicitor> GetSolicitorByIdAsync(string solicitorId);
        Task<IEnumerable<Solicitor>> GetAutocompletedSolicitorsAsync(string solicitorName);

    }
}
