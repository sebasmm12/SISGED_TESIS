namespace SISGED.Shared.Models.Responses.Dashboards;

public class DocumentsState
{
    public DocumentsState(string state, int count)
    {
        State = state;
        Count = count;
    }

    public string State { get; set; }

    public int Count { get; set; }
}