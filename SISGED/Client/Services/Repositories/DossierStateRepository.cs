using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DossierStateRepository : IDossierStateRepository
    {
        private readonly IDictionary<string, Color> _dossierStateColors = new Dictionary<string, Color>
        {
            { "solicitado", Color.Primary },
            { "en proceso", Color.Warning },
            { "finalizado", Color.Success },
        };
            
        public Color GetDossierStateColor(string dossierState)
        {
            dossierState = dossierState.ToLower();

            return _dossierStateColors.FirstOrDefault(documentStateColor => documentStateColor.Key == dossierState, new("defecto", Color.Dark)).Value;
        }

        public IEnumerable<SelectOption> GetDossierStates()
        {
            return new List<SelectOption>()
            {
                new("Solicitado", "solicitado"),
                new("En proceso", "en proceso"),
                new("Finalizado", "finalizado")
            };
        }
    }
}
