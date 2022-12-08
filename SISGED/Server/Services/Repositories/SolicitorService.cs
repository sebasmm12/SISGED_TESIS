using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using System.Collections.Generic;

namespace SISGED.Server.Services.Repositories
{
    public class SolicitorService : ISolicitorService
    {

        private readonly IMongoCollection<Solicitor> _solicitorCollection;
        public string CollectionName => "notarios";
        public SolicitorService(IMongoDatabase mongoDatabase)
        {
            _solicitorCollection = mongoDatabase.GetCollection<Solicitor>(CollectionName);
        }

        public async Task<IEnumerable<Solicitor>> GetAutocompletedSolicitorsAsync(string? solicitorName, bool? exSolicitor)
        {
            var autocompletedSolicitors = await _solicitorCollection.Aggregate<Solicitor>(GetAutocompletedSolicitorPipeline(solicitorName, exSolicitor)).ToListAsync();

            if (autocompletedSolicitors is null) throw new Exception($"No se pudo encontrar ningun notario con el nombre {solicitorName}");

            return autocompletedSolicitors;
        }

        public async Task<Solicitor> GetSolicitorByIdAsync(string solicitorId)
        {
            var solicitor = await _solicitorCollection.Find(solicitor => solicitor.Id == solicitorId).FirstOrDefaultAsync();

            if (solicitor is null) throw new Exception($"No se pudo encontrar ningun notario con el identificador {solicitorId}");

            return solicitor;
        }

        private static BsonDocument[] GetAutocompletedSolicitorPipeline(string? solicitorName, bool? exSolicitor)
        {
            var addFieldAggregation = MongoDBAggregationExtension.AddFields(new()
            {
                { "fullName", MongoDBAggregationExtension.Concat(new List<BsonValue>() { "$nombre", " ", "$apellido" }) }
            });

            var matchAggregation = GetAutocompletedSolicitorMatch(solicitorName, exSolicitor);

            var unSetAggregation = MongoDBAggregationExtension.UnSet("fullName");

            return new BsonDocument[] { addFieldAggregation, matchAggregation, unSetAggregation };
        }

        private static BsonDocument GetAutocompletedSolicitorMatch(string? solicitorName, bool? exSolicitor)
        {
            var solicitorMatchDictionary = new Dictionary<string, BsonValue>()
            {
                { "fullName", MongoDBAggregationExtension.Regex(solicitorName?.Trim().ToLower() + ".*", "i") } 
            };

            if (exSolicitor is not null) solicitorMatchDictionary.Add("exnotario", exSolicitor.Value);

            var matchAggregation = MongoDBAggregationExtension.Match(solicitorMatchDictionary);

            return matchAggregation;
        }
    }
}
