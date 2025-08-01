using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.DTOs;

public class RoleDocumentDTO
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonElement("role")]
    public string Role { get; set; } = default!;

    [BsonElement("state")]
    public string State { get; set; } = default!;

    [BsonElement("creationDate")]
    public DateTime CreationDate { get; set; }

    [BsonElement("endDate")]
    public DateTime? EndDate { get; set; }

    [BsonElement("dueDate")]
    public DateTime DueDate { get; set; }
}