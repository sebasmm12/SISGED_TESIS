using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.PublicDeed;
using SISGED.Shared.Models.Responses.PublicDeed;
using System.Text.RegularExpressions;

namespace SISGED.Server.Services.Repositories
{
    public class PublicDeedsService : IPublicDeedsService
    {
        private readonly IMongoCollection<PublicDeed> _publicdeed;
        private readonly IMongoCollection<Solicitor> _solicitor;
        public string CollectionName => "asistente";
        public string CollectionNameAux => "asistente";
        public PublicDeedsService(IMongoDatabase mongoDatabase)
        {
            _publicdeed = mongoDatabase.GetCollection<PublicDeed>(CollectionName);
            _solicitor = mongoDatabase.GetCollection<Solicitor>(CollectionNameAux);
        }
        public async Task<IEnumerable<PublicDeed>> Filter(string term)
        {
            string regex = "\\b" + term.ToLower() + ".*";
            var filter = Builders<PublicDeed>.Filter.Regex("titulo", new BsonRegularExpression(regex, "i"));
            return await _publicdeed.Find(filter).ToListAsync();
        }

        public async Task<PublicDeed> GetById(string id)
        {
            PublicDeed publicdeed = await _publicdeed.Find(deed => deed.Id == id).FirstOrDefaultAsync();
            return publicdeed;
        }

        public async Task<IEnumerable<PublicDeedFilterResponse>> SpecialFilter(PublicDeedSearchParametersFullFilterRequest searchparameters)
        {
            var filtered = await _publicdeed.Aggregate<PublicDeedFilterResponse>(GetFilteredPublicDeedPipeline(searchparameters))
                                                        .ToListAsync();

            if (filtered is null) throw new Exception("No se ha podido encontrar escrituras públicas con esos filtros.");

            return filtered;
        }

        public async Task<UpdateResult> updatePublicDeedBySignatureConclusion(PublicDeed pd)
        {
            var filter = Builders<PublicDeed>.Filter.Eq(deed => deed.Id, pd.Id);

            var update = Builders<PublicDeed>.Update.Set(deed => deed.State, "concluido");

            var publicdeed = await _publicdeed.UpdateOneAsync(filter, update);
            return publicdeed;
        }

        #region private methods
        private static BsonDocument[] GetFilteredPublicDeedPipeline(PublicDeedSearchParametersFullFilterRequest param)
        {
            var lookUpAggregation = GetFilteredLookUpPipeline();

            //Creación del BsonDocument del filtros de busqueda
            var filteredDoc =  SearchFilterBsonDocCreation(param);

            var unWindAggregation = MongoDBAggregationExtension.UnWind(new("$notario"));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "Id", "$_id" },
                { "OfficeDirection", "$direccionoficio" },
                { "NotaryId", "$idnotario" },
                { "LegalActs", "$actosjuridicos" },
                { "PublicDeedDate", "$fechaescriturapublica" },
                { "Url", "$url" },
                { "State", "$estado" },
                { "Notary", MongoDBAggregationExtension.Concat(new List<BsonValue>() { "$notario.nombre", " ", "$notario.apellido" }) },
                { "Title", "$titulo" }
            });

            var matchAggregation = MongoDBAggregationExtension.Match(filteredDoc);

            return new BsonDocument[] { lookUpAggregation, unWindAggregation, projectAggregation, matchAggregation };

        }

        private static BsonDocument SearchFilterBsonDocCreation(PublicDeedSearchParametersFullFilterRequest param)
        {
            var doc = new BsonDocument();
            if (param.NotarialOfficeDirection != null & param.NotarialOfficeDirection != "")
            {
                doc.Add("direccionoficio",
                                   new BsonDocument("$regex", param.NotarialOfficeDirection + ".*")
                                   .Add("$options", "i"));
            }
            if (param.NotaryName != null & param.NotaryName != null)
            {
                doc.Add("notario",
                                    new BsonDocument("$regex", param.NotaryName + ".*")
                                    .Add("$options", "i"));
            }
            if (param.LegalAct != null & param.LegalAct != "")
            {
                doc.Add("actosjuridicos.titulo",
                                    new BsonDocument("$regex", param.LegalAct + ".*")
                                    .Add("$options", "i"));
            }
            if (param.GrantersName != null)
            {
                if (param.GrantersName.Count != 0)
                {
                    var regexGrantersList = param.GrantersName.Select(o => new Regex(o + ".*")).ToList();
                    doc.Add("actosjuridicos.otorgantes.nombre",
                                        new BsonDocument("$in", new BsonArray().AddRange(regexGrantersList)));
                }

            }
            return doc;
        }

        private static BsonDocument GetFilteredLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "notariox", MongoDBAggregationExtension.ObjectId("$idnotario") }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$notariox") })))
            };

            return MongoDBAggregationExtension.Lookup(new("notarios", letPipeline, lookUpPipeline, "notario"));
        }
        #endregion
    }
}
