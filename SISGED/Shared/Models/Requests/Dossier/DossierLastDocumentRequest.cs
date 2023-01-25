using SISGED.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace SISGED.Shared.Models.Requests.Dossier
{
    public class DossierLastDocumentRequest
    {
        [Required(ErrorMessage = "Debe ingresar el identificador del expediente")]
        public string Id { get; set; } = default!;
        [Required(ErrorMessage = "Debe especificar el identificador del documento")]
        public string DocumentId { get; set; } = default!;
        [Required(ErrorMessage = "No existe derivación para registrarlo en el expediente")]
        public Derivation Derivation { get; set; } = default!;

        public DossierLastDocumentRequest()
        {
            
        }

        public DossierLastDocumentRequest(string id, string documentId, Derivation derivation)
        {
            Id = id;
            DocumentId = documentId;
            Derivation = derivation;
        }
    }
}
