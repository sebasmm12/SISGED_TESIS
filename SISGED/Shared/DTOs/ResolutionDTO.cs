using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class ResolutionDTO
    {
        public DocumentTypeInfoResponse Penalty { get; set; } = default!;
        public Client Client { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        public ResolutionDTO() { }

        public ResolutionDTO(Client client, AutocompletedSolicitorResponse solicitor, DocumentTypeInfoResponse penalty)
        {
            Client = client;
            Solicitor = solicitor;
            Penalty = penalty;
        }
    }
}
