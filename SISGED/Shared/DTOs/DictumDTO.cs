using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class DictumDTO : ComplaintDocumentDTO
    {
        public DictumDTO(Client client, AutocompletedSolicitorResponse solicitor) : base(client, solicitor)
        {
            
        }
    }
}
