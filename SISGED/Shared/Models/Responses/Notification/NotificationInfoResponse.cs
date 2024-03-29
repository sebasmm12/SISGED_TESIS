﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SISGED.Shared.Models.Responses.Notification
{
    public class NotificationInfoResponse
    {
        public NotificationInfoResponse(string id, string description, string senderUserImage, bool seen, string link, DateTime issueDate)
        {
            Id = id;
            Description = description;
            SenderUserImage = senderUserImage;
            Seen = seen;
            Link = link;
            IssueDate = issueDate;
        }

        public NotificationInfoResponse() { }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("senderUserImage")]
        public string SenderUserImage { get; set; } = default!;
        [BsonElement("seen")]
        public bool Seen { get; set; }
        [BsonElement("link")]
        public string Link { get; set; } = default!;
        [BsonElement("issueDate")]
        public DateTime IssueDate { get; set; }
    }
}
