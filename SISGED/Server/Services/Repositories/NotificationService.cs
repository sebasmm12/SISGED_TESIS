using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Notification;

namespace SISGED.Server.Services.Repositories
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<Notification> _notificationsCollection;

        public string CollectionName => "notificaciones";

        public NotificationService(IMongoDatabase mongoDatabase)
        {
            _notificationsCollection = mongoDatabase.GetCollection<Notification>(CollectionName);
        }

        public async Task<IEnumerable<NotificationInfoResponse>> GetNotificationsByUserIdAsync(string userId)
        {
            var notifications = await _notificationsCollection.Aggregate<NotificationInfoResponse>(GetNotificationByUserIdPipeline(userId)).ToListAsync();

            if (notifications is null) throw new Exception($"No se pudo encontrar las notificaciones del usuario con identificador { userId }");

            return notifications;
        }

        public async Task<Notification> GetNotificationAsync(string notificationId)
        {
            var notification = await _notificationsCollection.Find(notification => notification.Id == notificationId).FirstOrDefaultAsync();

            if (notification is null) throw new Exception($"No se pudo encontrar la notificación con identificador { notificationId }");

            return notification;
        }

        public async Task<Notification> UpdateNotificationAsync(Notification notification)
        {
            var notificationQuery = Builders<Notification>.Filter.Eq(notification => notification.Id, notification.Id);

            var notificationUpdate = Builders<Notification>.Update.Set("visto", !notification.Seen);

            var updatedNotification = await _notificationsCollection.FindOneAndUpdateAsync(notificationQuery, notificationUpdate, new()
            {
                ReturnDocument = ReturnDocument.After
            });

            return updatedNotification;
        }

        #region private methods
        private BsonDocument[] GetNotificationByUserIdPipeline(string userId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("idreceptor", userId));

            var lookupAggregation = GetUsersLookUpPipeline();

            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$user"));

            var sortAggregation = MongoDBAggregationExtension.Sort(new Dictionary<string, BsonValue>() { { "visto", 1 }, { "fechaemision", -1 } });

            var limitAggregation = MongoDBAggregationExtension.Limit(10);

            var projectAggregation = GetNotificationByUserIdProject();

            return new BsonDocument[] { matchAggregation, sortAggregation, limitAggregation, lookupAggregation, unwindAggregation, projectAggregation };
        }

        private static BsonDocument GetNotificationByUserIdProject()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "description", "$cuerpo" },
                { "senderUserImage", "$user.datos.imagen" },
                { "seen", "$visto" },
                { "link", "$enlace" },
                { "issueDate", "$fechaemision" }
            });

            return projectAggregation;
        }

        private static BsonDocument GetUsersLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userId", MongoDBAggregationExtension.ObjectId("$idemisor")  }
            }; 

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$userId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("usuarios", letPipeline, lookUpPipeline, "user"));
        }

        #endregion
    }
}
