namespace SISGED.Shared.Models.Responses.Dashboards;

public class RoleDocumentsResponse
{
    public RoleDocumentsResponse(string role, Dictionary<string, int> documents)
    {
        Role = role;
        Documents = documents;
    }

    public RoleDocumentsResponse() { }

    public string Role { get; set; } = default!;

    public IDictionary<string, int> Documents { get; set; } = default!;
}