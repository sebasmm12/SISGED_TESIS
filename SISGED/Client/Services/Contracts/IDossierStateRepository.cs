using MudBlazor;
using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IDossierStateRepository
    {
        IEnumerable<SelectOption> GetDossierStates();
        Color GetDossierStateColor(string dossierState);
    }
}
