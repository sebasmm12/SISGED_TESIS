using MongoDB.Bson;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Helpers.Infrastructure
{
    public static class MongoDBAggregationExtension
    {
        public static BsonDocument Expr(BsonValue bsonElements)
        {

            return new BsonDocument("$expr", bsonElements);
        }

        public static BsonDocument Eq(BsonArray bsonArray)
        {

            return new BsonDocument("$eq", bsonArray);
        }

        public static BsonDocument Match(BsonValue bsonElements)
        {
            return new BsonDocument("$match", bsonElements);
        }

        public static BsonDocument Match(Dictionary<string, string> elements)
        {
            return new BsonDocument("$match", new BsonDocument().AddRange(elements));
        }

        public static BsonDocument Lookup(LookUpAggregationDTO lookUpDTO)
        {
            var lookUpAggregation = new BsonDocument("$lookup", new BsonDocument()
            {
                new BsonElement("from", lookUpDTO.FromNameCollection),
                new BsonElement("let", new BsonDocument().AddRange(lookUpDTO.Let)),
                new BsonElement("pipeline", lookUpDTO.Pipelines),
                new BsonElement("as", lookUpDTO.ResultName)
            });

            return lookUpAggregation;
        }

        public static BsonDocument UnWind(UnwindAggregationDTO unwindAggregationDTO)
        {
            var unWindAggregation = new BsonDocument
            {
                { "path", unwindAggregationDTO.Path }
            };

            if (!string.IsNullOrEmpty(unwindAggregationDTO.IncludeArrayIndex)) unWindAggregation.Add("includeArrayIndex", unwindAggregationDTO.IncludeArrayIndex);

            if (unwindAggregationDTO.PreserveNullAndEmptyArrays.HasValue) unWindAggregation.Add("preserveNullAndEmptyArrays", unwindAggregationDTO.PreserveNullAndEmptyArrays);

            return new BsonDocument("$unwind", unWindAggregation);
        }

        public static BsonDocument Project(Dictionary<string, BsonValue> elements)
        {
            return new BsonDocument("$project", new BsonDocument().AddRange(elements));
        }
    }
}
