namespace SISGED.Shared.DTOs;

public class SessionResolutionContentDTO
{
    public string Title { get; set; } = default!;

    public string ComplaintId { get; set; } = default!;

    public string SolicitorId { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string PreviousDocumentId { get; set; } = default!;
}