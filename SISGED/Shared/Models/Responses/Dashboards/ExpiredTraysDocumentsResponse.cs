namespace SISGED.Shared.Models.Responses.Dashboards;

public class ExpiredTraysDocumentsResponse
{
    public ExpiredTraysDocumentsResponse()
    {
        
    }

    public ExpiredTraysDocumentsResponse(IEnumerable<ExpiredTrayDocuments> expiredTrayDocuments)
    {
        ExpiredTrayDocuments = expiredTrayDocuments;
    }

    public IEnumerable<ExpiredTrayDocuments> ExpiredTrayDocuments { get; set; } = default!;
}