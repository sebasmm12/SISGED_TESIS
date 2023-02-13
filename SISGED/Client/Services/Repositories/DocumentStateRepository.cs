using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DocumentStateRepository : IDocumentStateRepository
    {
        private readonly IDictionary<string, DocumentState> _documentStateColors = new Dictionary<string, DocumentState>
        {
            { "registrado", new(Color.Primary, "Registro del Documento")   },
            { "modificado", new(Color.Warning, "Modificación del Documento") },
            { "generado",   new(Color.Info, "Generación del Documento") },
            { "derivado",   new(Color.Success, "Derivación del Documento") },
            { "evaluado",   new(Color.Secondary, "Evaluación del Documento") },
            { "anulado",    new(Color.Error, "Anulación del Documento") },
        };

        public DocumentState GetDocumentState(string documentState)
        {
            documentState = documentState.ToLower();

            return _documentStateColors.FirstOrDefault(documentStateColor => 
                        documentStateColor.Key == documentState, new("defecto", new(Color.Dark, "Modificación del Documento"))).Value;
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
