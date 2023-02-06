using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DocumentStateRepository : IDocumentStateRepository
    {
        private readonly IDictionary<string, Color> _documentStateColors = new Dictionary<string, Color>
        {
            { "registrado", Color.Primary },
            { "modificado", Color.Warning },
            { "generado", Color.Info },
            { "derivado", Color.Success },
            { "evaluado", Color.Secondary },
            { "anulado", Color.Error },
        };

        public Color GetDocumentStateColor(string documentState)
        {
            documentState = documentState.ToLower();

            return _documentStateColors.FirstOrDefault(documentStateColor => documentStateColor.Key == documentState, new("defecto", Color.Dark)).Value;
        }

        public IEnumerable<SelectOption> GetDocumentStates()
        {
            return new List<SelectOption>()
            {
                new("Registrado", "registrado"),
                new("Modificado", "modificado"),
                new("Generado", "generado"),
                new("Derivado", "derivado"),
                new("Anulado", "anulado"),
                new("Evaluado", "evaluado")
            };
        }
    }
}
