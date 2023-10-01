using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Document
{
    public class SolicitorDesignationDocumentResponse : Entities.Document
    {
        public Evaluation Evaluation { get; set; } = new Evaluation();
        public SolicitorDesignationDocumentResponseContent content { get; set; } = new SolicitorDesignationDocumentResponseContent();
    }
    public class SolicitorDesignationDocumentResponseContent
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime FulfillmentDate { get; set; }
        public string SolicitorOfficeLocation { get; set; }
        public string UserId { get; set; }
        public Entities.Solicitor SolicitorId { get; set; }
        public List<string> URLannex { get; set; } = new();
    }
}
