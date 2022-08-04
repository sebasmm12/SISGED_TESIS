using MongoDB.Bson;

namespace SISGED.Shared.DTOs
{
    public class LookUpAggregationDTO
    {
        public LookUpAggregationDTO(string fromNameCollection, Dictionary<string, BsonValue> let, BsonArray pipelines, string resultName)
        {
            FromNameCollection = fromNameCollection;
            Let = let;
            Pipelines = pipelines;
            ResultName = resultName;
        }

        public string FromNameCollection { get; set; } = default!;
        public Dictionary<string, BsonValue> Let { get; set; } = default!;
        public BsonArray Pipelines { get; set; } = default!;
        public string ResultName { get; set; } = default!;
    }
}
