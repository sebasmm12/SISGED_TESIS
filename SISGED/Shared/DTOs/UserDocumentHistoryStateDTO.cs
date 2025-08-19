using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.DTOs;

public class UserDocumentHistoryStateDTO
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public string State { get; set; } = default!;

    public DateTime? EndDate { get; set; } = default!;

    public DateTime DueDate { get; set; } = default!;
    
    public bool? IsApproved { get; set; } = default!;

    public string DateFilter { get; set; } = default!;
}