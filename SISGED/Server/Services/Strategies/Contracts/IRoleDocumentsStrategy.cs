using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Strategies.Contracts;

public interface IRoleDocumentsStrategy
{
    DateFilterDTO DateFilter { get; }

    Task<IEnumerable<RoleDocumentDTO>> GetDocumentsAsync();
}