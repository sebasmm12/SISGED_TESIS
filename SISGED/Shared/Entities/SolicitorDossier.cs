﻿using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class SolicitorDossier
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("documentName")]
        public string DocumentName { get; set; } = default!;
        [BsonElement("issueDate")]
        public DateTime IssueDate { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = default!;
    }
}
