using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs;

public class SessionResolutionDTO : ComplaintDocumentDTO
{
    public DocumentContentDTO PreviousDocumentContent { get; set; } 


    public SessionResolutionDTO(
        Client client, 
        AutocompletedSolicitorResponse solicitor,
        DocumentContentDTO previousDocumentContent) : base(client, solicitor)
    {
        PreviousDocumentContent = previousDocumentContent;
    }
}