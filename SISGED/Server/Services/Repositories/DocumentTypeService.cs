using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Repositories
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IMongoCollection<DocumentType> _documentTypesCollection;
        public string CollectionName => "tipoDocumentos";

        public DocumentTypeService(IMongoDatabase mongoDatabase)
        {
            _documentTypesCollection = mongoDatabase.GetCollection<DocumentType>(CollectionName);
        }

        public async Task<IEnumerable<DocumentType>> GetDocumentTypesAsync(string type)
        {
            var documentTypes = await _documentTypesCollection.Find(documentType => documentType.Type == type).ToListAsync();

            if (documentTypes is null) throw new Exception($"No se pudo obtener los tipos de documentos mediante el tipo { type }");

            return documentTypes;
        }

        public async Task<DocumentType> GetDocumentTypeAsync(string id)
        {
            var documentType = await _documentTypesCollection.Find(documentType => documentType.Id == id).FirstOrDefaultAsync();

            if (documentType is null) throw new Exception($"No se pudo obtener el tipo de documento mediante el id {id}");

            return documentType;
        }
    }
}
