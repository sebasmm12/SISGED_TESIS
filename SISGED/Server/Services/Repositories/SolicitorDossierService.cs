using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Document;
using SISGED.Shared.Models.Queries.SolicitorDossier;
using SISGED.Shared.Models.Responses.SolicitorDossier;

namespace SISGED.Server.Services.Repositories
{
    public class SolicitorDossierService : ISolicitorDossierService
    {
        private readonly IMongoCollection<SolicitorDossier> _solicitorDossierCollection;

        public string CollectionName => "expedientesnotarios";

        public SolicitorDossierService(IMongoDatabase mongoDatabase)
        {
            _solicitorDossierCollection = mongoDatabase.GetCollection<SolicitorDossier>(CollectionName);
        }

        public async Task<IEnumerable<SolicitorDossier>> GetSolicitorsDossiersAsync(SolicitorDossierPaginationQuery solicitorDossierPaginationQuery)
        {

            var solicitorDossiers = await _solicitorDossierCollection.Aggregate<SolicitorDossier>(GetPaginatedSolicitorDossiersPipeline(solicitorDossierPaginationQuery))
                                                                     .ToListAsync();
                    

            if (solicitorDossiers is null) throw new Exception($"No se pudo obtener los expedientes del notario con identificador {solicitorDossierPaginationQuery.SolicitorId}");

            return solicitorDossiers;
        }

        public async Task<int> CountSolicitorDossiersAsync(SolicitorDossierPaginationQuery solicitorDossierPaginationQuery)
        {
            var totalSolicitorDossiers = await _solicitorDossierCollection.Aggregate<SolicitorDossierCounterDTO>(GetTotalSolicitorDossiersPipeline(solicitorDossierPaginationQuery))
                                                                          .FirstOrDefaultAsync();
            return totalSolicitorDossiers.Total;
        }

        public async Task<IEnumerable<int>> GetSolicitorDossierAvailableYearsAsync(string solicitorId)
        {
            var solicitorDossierYears = await _solicitorDossierCollection.Aggregate<SolicitorDossierYearInfoDTO>(GetSolicitorDossierYearsPipeline(solicitorId))
                                                                         .FirstOrDefaultAsync();

            if (solicitorDossierYears is null) throw new Exception($"No se pudo obtener los años de los expedientes del notario con identificador {solicitorId}");

            return solicitorDossierYears.Years;
        }

        private static BsonDocument[] GetSolicitorDossierYearsPipeline(string solicitorId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("idnotario", solicitorId));

            var addFieldAggregation = MongoDBAggregationExtension.AddFields(new()
            {
                { "issueYear", MongoDBAggregationExtension.Year("$fechaexpedicion") }
            });

            var groupAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$idnotario" },
                { "years", MongoDBAggregationExtension.AddToSet("$issueYear") }
            });

            var unsetAggregation = MongoDBAggregationExtension.UnSet("_id");

            return new BsonDocument[] { matchAggregation, addFieldAggregation, groupAggregation, unsetAggregation };

        }

        private static BsonDocument[] GetSolicitorDossiersPipeline(SolicitorDossierPaginationQuery solicitorDossierPaginationQuery)
        {
            var addFieldAggregation = MongoDBAggregationExtension.AddFields(new()
            {
                { "issueYear", MongoDBAggregationExtension.Year("$fechaexpedicion")  }
            });

            var matchAggregation = GetSolicitorDossierQueryMatch(solicitorDossierPaginationQuery);

            var unsetAggregation = MongoDBAggregationExtension.UnSet("issueYear");
            
            return new BsonDocument[] { addFieldAggregation, matchAggregation, unsetAggregation };
        }

        private static BsonDocument[] GetPaginatedSolicitorDossiersPipeline(SolicitorDossierPaginationQuery solicitorDossierPaginationQuery)
        {

            var solicitorDossiersAggregation = GetSolicitorDossiersPipeline(solicitorDossierPaginationQuery);

            var skipAggregation = MongoDBAggregationExtension.Skip(solicitorDossierPaginationQuery.Page * solicitorDossierPaginationQuery.PageSize);

            var limitAggregation = MongoDBAggregationExtension.Limit(solicitorDossierPaginationQuery.PageSize);
            
            return solicitorDossiersAggregation.Concat(new BsonDocument[] { skipAggregation, limitAggregation }).ToArray();
        }

        private static BsonDocument[] GetTotalSolicitorDossiersPipeline(SolicitorDossierPaginationQuery solicitorDossierPaginationQuery)
        {
            var solicitorDossiersAggregation = GetSolicitorDossiersPipeline(solicitorDossierPaginationQuery);

            var countAggregation = MongoDBAggregationExtension.Count("total");

            return solicitorDossiersAggregation.Concat(new BsonDocument[] { countAggregation }).ToArray();
        }

        private static BsonDocument GetSolicitorDossierQueryMatch(SolicitorDossierPaginationQuery solicitorDossierPaginationQuery)
        {
            var matchedElements = new Dictionary<string, BsonValue>()
            {
                { "idnotario", solicitorDossierPaginationQuery.SolicitorId }
            };

            if (solicitorDossierPaginationQuery.Years.Any()) matchedElements.Add("issueYear", MongoDBAggregationExtension.In(solicitorDossierPaginationQuery.Years));

            var matchAggregation = MongoDBAggregationExtension.Match(matchedElements);

            return matchAggregation;
        }
    }
}
