using MudBlazor;
using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IDocumentStateRepository
    {
        IEnumerable<SelectOption> GetDocumentStates();
        DocumentState GetDocumentState(string documentState);
    }
}
