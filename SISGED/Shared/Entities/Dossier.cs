using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Dossier
    {
        public Dossier() {  }

        public Dossier(Client client, string type, string state)
        {
            Client = client;
            Type = type;
            State = state;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("cliente")]
        public Client Client { get; set; } = default!;
        [BsonElement("fechainicio")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("fechafin")]
        public DateTime? EndDate { get; set; }
        [BsonElement("documentos")]
        public List<DossierDocument> Documents { get; set; } = new();
        [BsonElement("derivaciones")]
        public List<Derivation> Derivations { get; set; } = new();
        [BsonElement("estado")]
        public string State { get; set; } = default!;


        public void AddDocument(DossierDocument dossierDocument)
        {
            Documents.Add(dossierDocument);
        }
    }
}
