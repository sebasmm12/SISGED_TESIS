using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Dashboards;

public class ExpiredTrayDocuments
{
    [BsonElement("dossierId")]
    public string DossierId { get; set; } = default!;

    [BsonElement("documentId")]
    public string DocumentId { get; set; } = default!;

    [BsonElement("client")]
    public string Client { get; set; } = default!;

    [BsonElement("dossierType")]
    public string DossierType { get; set; } = default!;

    [BsonElement("documentType")] 
    public string DocumentType { get; set; } = default!;

    [BsonElement("expirationDays")]
    public int ExpirationDays { get; set; }
}