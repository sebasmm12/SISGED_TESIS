﻿using MongoDB.Bson;
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

        public static BsonDocument Filter(BsonValue bsonElements, BsonValue bsonFilter)
        {

            return new BsonDocument("$filter", new BsonDocument("input", bsonElements)
                                                     .Add("as", "item")
                                                     .Add("cond", bsonFilter));
        }

        public static BsonDocument Match(BsonValue bsonElements)
        {
            return new BsonDocument("$match", bsonElements);
        }

        public static BsonDocument Match(Dictionary<string, BsonValue> elements)
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

        public static BsonDocument ObjectId(string id)
        {
            return new BsonDocument("$toObjectId", id);
        }

        public static BsonDocument Concat(IEnumerable<BsonValue> valuesToConcat)
        {
            return new BsonDocument("$concat", new BsonArray().AddRange(valuesToConcat));
        }

        public static BsonDocument Regex(string regexPatter, string? options = null)
        {
            var regexAggregation = new BsonDocument
            {
                { "$regex", regexPatter }
            };

            if (!string.IsNullOrEmpty(options)) regexAggregation.Add("$options", options);

            return regexAggregation;
        }

        public static BsonDocument Group(Dictionary<string, BsonValue> valuesToGroup)
        {
            return new BsonDocument("$group", new BsonDocument().AddRange(valuesToGroup));
        }
        public static BsonDocument Sum(BsonValue value)
        {
            return new BsonDocument("$sum", value);
        }

        public static BsonDocument First(BsonValue value)
        {
            return new BsonDocument("$first", value);
        }

        public static BsonDocument Push(BsonValue value)
        {
            return new BsonDocument("$push", value);
        }

        public static BsonDocument Month(BsonValue date)
        {
            return new BsonDocument("$month", date);
        }

        public static BsonDocument Cond(BsonValue condition, BsonValue trueValue, BsonValue falseValue)
        {
            return new BsonDocument("$cond", new BsonArray().Add(condition).Add(trueValue).Add(falseValue));
        }

        public static BsonDocument In(BsonValue value, BsonArray values)
        {
            return new BsonDocument("$in", new BsonArray().Add(value).Add(values));
        }

        public static BsonDocument NotIn(IEnumerable<BsonValue> valuesToExclude)
        {
            return new BsonDocument("$nin", new BsonArray().AddRange(valuesToExclude));
        }

        public static BsonDocument Switch(IEnumerable<BsonValue> cases, BsonValue defaultValue)
        {
            return new BsonDocument("$switch", new BsonDocument().Add("branches", new BsonArray().AddRange(cases)).Add("default", defaultValue));
        }

        public static BsonDocument Case(BsonValue condition, BsonValue result)
        {
            return new BsonDocument().Add("case", condition).Add("then", result);
        }

        public static BsonDocument ArrayElementAt(IEnumerable<BsonValue> values)
        {
            return new BsonDocument("$arrayElemAt", new BsonArray().AddRange(values));
        }

        public static BsonDocument Skip(int totalRecordsToSkip)
        {
            return new BsonDocument("$skip", totalRecordsToSkip);
        }

        public static BsonDocument Limit(int totalRecordsToTake)
        {
            return new BsonDocument("$limit", totalRecordsToTake);
        }
    }
}
