using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Dossier
    {
        public Dossier() {  }

        public Dossier(string type, string state)
        {
            Type = type;
            State = state;
        }

        public Dossier(Client client, string type, string state) : this(type, state)
        {
            Client = client;
        }

        public Dossier(string id, string type, string state) : this(type, state)
        {
            Id = id;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("client")]
        public Client Client { get; set; } = default!;
        [BsonElement("startDate")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("endDate")]
        public DateTime? EndDate { get; set; }
        [BsonElement("documents")]
        public List<DossierDocument> Documents { get; set; } = new();
        [BsonElement("documentsHistory")]
        public List<DossierDocument> DocumentsHistory { get; set; } = new();
        [BsonElement("derivations")]
        public List<Derivation> Derivations { get; set; } = new();
        [BsonElement("state")]
        public string State { get; set; } = default!;


        public void AddDocument(DossierDocument dossierDocument)
        {
            Documents.Add(dossierDocument);
        }

        public void AddDocumentHistory(DossierDocument dossierDocument)
        {
            DocumentsHistory.Add(dossierDocument);
        }
    }
}
