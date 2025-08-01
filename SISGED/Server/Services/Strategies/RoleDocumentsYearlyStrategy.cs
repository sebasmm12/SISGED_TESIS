using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Strategies;

public class RoleDocumentsYearlyStrategy : IRoleDocumentsStrategy
{
    private readonly IDocumentService _documentService;

    public RoleDocumentsYearlyStrategy(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public DateFilterDTO DateFilter => DateFilterDTO.Yearly;

    public async Task<IEnumerable<RoleDocumentDTO>> GetDocumentsAsync()
    {
        var documents = await _documentService.GetRoleDocumentsYearlyAsync();

        return documents;
    }
}