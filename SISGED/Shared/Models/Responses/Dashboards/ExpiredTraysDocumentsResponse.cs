namespace SISGED.Shared.Models.Responses.Dashboards;

public class ExpiredTraysDocumentsResponse
{
    public IEnumerable<ExpiredTrayDocuments> ExpiredInputTrayDocuments { get; set; } = default!;

    public IEnumerable<ExpiredTrayDocuments> ExpiredOutputTrayDocuments { get; set; } = default!;
}