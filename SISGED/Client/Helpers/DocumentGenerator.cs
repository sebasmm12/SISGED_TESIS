using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Client.Helpers
{
    public class DocumentGenerator
    {
        public DocumentResponse Document { get; set; } = default!;
        public string DocumentCode { get; set; } = default!;

        public DocumentGenerator()
        {
            
        }

        public DocumentGenerator(DocumentResponse document, string documentCode)
        {
            Document = document;
            DocumentCode = documentCode;
        }
    }
}
