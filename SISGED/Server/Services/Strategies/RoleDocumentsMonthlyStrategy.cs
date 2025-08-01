using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Strategies;

public class RoleDocumentsMonthlyStrategy : IRoleDocumentsStrategy
{
    private readonly IDocumentService _documentService;

    public RoleDocumentsMonthlyStrategy(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public DateFilterDTO DateFilter => DateFilterDTO.Monthly;

    public async Task<IEnumerable<RoleDocumentDTO>> GetDocumentsAsync()
    {
        var documents = await _documentService.GetRoleDocumentsMonthlyAsync();

        return documents;
    }
}