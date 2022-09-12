using SISGED.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace SISGED.Shared.Models.Requests.Dossier
{
    public class DossierLastDocumentRequest
    {
        [Required(ErrorMessage = "Debe ingresar el identificador del expediente")]
        public string Id { get; set; } = default!;
        [Required(ErrorMessage = "No existe documentos para registrarlo en el expediente")]
        [MinLength(1, ErrorMessage = "Debe registrar por lo menos un documento para registrarlo en el expediente")]
        public List<DossierDocument> Documents { get; set; } = default!;
        [Required(ErrorMessage = "No existe derivación para registrarlo en el expediente")]
        [MinLength(1, ErrorMessage = "Debe registrar por lo menos un registro de deribación para registrarlo en el expediente")]
        public List<Derivation> Derivations { get; set; } = default!;
    }
}
