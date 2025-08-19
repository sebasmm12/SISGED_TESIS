namespace SISGED.Shared.Models.Responses.Dashboards;

public class UserDocumentHistoryStateResponse
{
    public UserDocumentHistoryStateResponse()
    {
    }

    public UserDocumentHistoryStateResponse(string date, IDictionary<string, int> documents)
    {
        Date = date;
        Documents = documents;
    }

    public string Date { get; set; } = default!;

    public IDictionary<string, int> Documents { get; set; } = default!;
}