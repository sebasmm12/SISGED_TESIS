using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.DocumentVersion
{
    public class DocumentVersionInfo
    {
        public int Version { get; set; } = default!;
        public DateTime ModificationDate { get; set; }
        public string Url { get; set; } = default!;
    }
}
