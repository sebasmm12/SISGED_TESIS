using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Repositories
{
    public class DocumentVersionService : IDocumentVersionService
    {
        private readonly IMongoCollection<Document> _documentsCollection;

        public string CollectionName => "documentos";

        public DocumentVersionService(IMongoDatabase mongoDatabase)
        {
            _documentsCollection = mongoDatabase.GetCollection<Document>(CollectionName);
        }
        
        public async Task<IEnumerable<ContentVersion>> GetContentVersionsByDocumentIdAsync(string documentId)
        {
            var document = await _documentsCollection.Find(document => document.Id == documentId).FirstAsync();

            if (document is null) throw new Exception($"No se pudo encontrar el historial de version del documento con identificador { documentId }");

            return document.ContentsHistory;
        }
    }
}
