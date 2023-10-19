namespace SISGED.Shared.Models.Requests.Documents
{
    public class DossierWrapper
    {
        public string? Id { get; set; }
       
        public object Document { get; set; } = default!;
        
        public string? CurrentUserId { get; set; }
        
        public string? InputDocument { get; set; }

        public string? PreviousDocumentId { get; set; } = default!;

        public DossierWrapper(object document)
        {
            Document = document;
        }

        public DossierWrapper(string dossierId, object document, string previousDocumentId) : this(document)
        {
            Id = dossierId;
            PreviousDocumentId = previousDocumentId;
        }

        public DossierWrapper() {  }
    }
}
