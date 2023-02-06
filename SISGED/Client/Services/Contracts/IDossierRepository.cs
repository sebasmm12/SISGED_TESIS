using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IDossierRepository
    {
        IEnumerable<SelectOption> GetDossierTypes();
    }
}
