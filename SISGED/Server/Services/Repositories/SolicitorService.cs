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

        public async Task<IEnumerable<Solicitor>> GetAutocompletedSolicitorsAsync(string? solicitorName, bool? exSolicitor)
        {
            var solicitorRegularExpression = new BsonRegularExpression(solicitorName + ".*", "i");

            var solicitorFilter = Builders<Solicitor>.Filter.Regex(s => s.Name, solicitorRegularExpression)
               | Builders<Solicitor>.Filter.Regex(s => s.LastName, solicitorRegularExpression);

            if (exSolicitor is not null) solicitorFilter &= Builders<Solicitor>.Filter.Eq(s => s.ExSolicitor, exSolicitor.Value);

            var autocompletedSolicitors = await _solicitorCollection.Find(solicitorFilter).ToListAsync();

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
