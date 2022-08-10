using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

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

        public async Task<IEnumerable<Solicitor>> GetAutocompletedSolicitorsAsync(string solicitorName)
        {
            var autocompleteRegex = @"\b" + solicitorName + ".*";
            var autocompleteFilter = Builders<Solicitor>.Filter.Regex(s => s.Name, new BsonRegularExpression(autocompleteRegex, "i"));

            var autocompletedSolicitors = await _solicitorCollection.Find(autocompleteFilter).ToListAsync();

            if (autocompletedSolicitors is null) throw new Exception($"No se pudo encontrar ningun notario con el nombre {solicitorName}");

            return autocompletedSolicitors;
        }

        public async Task<Solicitor> GetSolicitorByIdAsync(string solicitorId)
        {
            var solicitor = await _solicitorCollection.Find(solicitor => solicitor.Id == solicitorId).FirstOrDefaultAsync();

            if (solicitor is null) throw new Exception($"No se pudo encontrar ningun notario con el identificador {solicitorId}");

            return solicitor;
        }
    }
}
