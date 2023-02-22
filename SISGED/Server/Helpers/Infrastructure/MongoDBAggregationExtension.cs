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

            if (unwindAggregationDTO.PreserveNullAndEmptyArrays.HasValue) unWindAggregation.Add("preserveNullAndEmptyArrays", unwindAggregationDTO.PreserveNullAndEmptyArrays.Value);

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

        public static BsonDocument In<T>(IEnumerable<T> values)
        {
            return new BsonDocument("$in", new BsonArray().AddRange(values));
        }

        public static BsonDocument In(BsonValue value, BsonValue values)
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

        public static BsonDocument Size(BsonValue value)
        {
            return new BsonDocument("$size", value);
        }

        public static BsonDocument Filter(string input, BsonValue cond, string? asName = null)
        {
            var filterPipeline = new BsonDocument().Add("input", input).Add("cond", cond);

            if (!string.IsNullOrEmpty(asName)) filterPipeline.Add("as", asName);

            return new BsonDocument("$filter", filterPipeline);
        }

        public static BsonDocument Let(BsonValue variables, BsonValue inExpression)
        {
            return new BsonDocument("$let", new BsonDocument().Add("vars", variables).Add("in", inExpression));
        }

        public static BsonDocument Sort(BsonValue bsonElement)
        {
            return new BsonDocument("$sort", bsonElement);
        }

        public static BsonDocument Sort(Dictionary<string, BsonValue> valuesToSort)
        {
            return new BsonDocument("$sort", new BsonDocument().AddRange(valuesToSort));
        }

        public static BsonDocument AddFields(Dictionary<string, BsonValue> valuesToAdd)
        {
            return new BsonDocument("$addFields", new BsonDocument().AddRange(valuesToAdd));
        }

        public static BsonDocument UnSet(BsonValue bsonElement)
        {
            return new BsonDocument("$unset", bsonElement);
        }

        public static BsonDocument UnSet(IEnumerable<BsonValue> elements)
        {
           return new BsonDocument("$unset", new BsonArray().AddRange(elements));
        }

        public static BsonDocument Set(Dictionary<string, BsonValue> valuesToSet)
        {
            return new BsonDocument("$set", new BsonDocument().AddRange(valuesToSet));
        }

        public static BsonDocument GetField(BsonValue field, BsonValue input)
        {
            return new BsonDocument("$getField", new BsonDocument().Add("field", field).Add("input",input));
        }

        public static BsonDocument Year(BsonValue date)
        {
            return new BsonDocument("$year", date);
        }

        public static BsonDocument AddToSet(BsonValue value)
        {
            return new BsonDocument("$addToSet", value);
        }

        public static BsonDocument Count(BsonValue value)
        {
            return new BsonDocument("$count", value);
        }

        public static BsonDocument Add(IEnumerable<BsonValue> values)
        {
            return new BsonDocument("$add", new BsonArray().AddRange(values));
        }

        public static BsonDocument ElementMatch(Dictionary<string, BsonValue> elementsToMatch)
        {
            return new BsonDocument("$elemMatch", new BsonDocument().AddRange(elementsToMatch));
        }

        public static BsonDocument GreaterThanEquals(BsonValue value)
        {
            return new BsonDocument("$gte", value);
        }

        public static BsonDocument GreaterThan(BsonValue value)
        {
            return new BsonDocument("$gt", value);
        }

        public static BsonDocument LessThanEquals(BsonValue value)
        {
            return new BsonDocument("$lte", value);
        }

        public static BsonDocument LessThan(BsonValue value)
        {
            return new BsonDocument("$lt", value);
        }
        
        public static BsonDocument ToString(BsonValue value)
        {
            return new BsonDocument("$toString", value);
        }
        
        public static BsonDocument ReplaceRoot(BsonValue value)
        {
            return new BsonDocument("$replaceRoot", new BsonDocument().Add("newRoot", value));
        }
    }
}
