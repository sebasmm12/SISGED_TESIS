using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Strategies;

public class RoleDocumentsDailyStrategy : IRoleDocumentsStrategy
{
    private readonly IDocumentService _documentService;

    public RoleDocumentsDailyStrategy(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public DateFilterDTO DateFilter => DateFilterDTO.Daily;

    public async Task<IEnumerable<RoleDocumentDTO>> GetDocumentsAsync()
    {
        var documents = await _documentService.GetRoleDocumentsDailyAsync();

        return documents;
    }
}