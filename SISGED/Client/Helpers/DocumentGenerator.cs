using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.DossierTray;

namespace SISGED.Client.Helpers
{
    public class DocumentGenerator
    {
        public DossierTrayResponse Dossier { get; set; } = default!;
        public DocumentResponse Document { get; set; } = default!;
        public string DocumentCode { get; set; } = default!;

        public DocumentGenerator()
        {
            
        }

        public DocumentGenerator(DocumentResponse document, string documentCode, DossierTrayResponse dossier = default!)
        {
            Document = document;
            DocumentCode = documentCode;
            Dossier = dossier;
        }
    }
}
