using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    public class StatisticsDTO
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public int mes { get; set; }
    }
    public class StatisticsDTOR
    {
        [BsonId]
        public string id { get; set; }
        public Int32 cantidad { get; set; }
    }

    public class StatisticsDTO4_project1
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public Evaluacion evaluacion { get; set; }
        public Int32 mes { get; set; }
    }

    public class StatisticsDTO4R
    {
        [BsonId]
        public string id { get; set; }
        public Int32 aprobados { get; set; }
        public Int32 rechazados { get; set; }
    }

    public class StatisticsDTO3_project
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public Int32 mes { get; set; }
    }

    public class StatisticsDTO3_group
    {
        [BsonId]
        public string id { get; set; }
        public Int32 caducados { get; set; }
    }
    /////////////////////////////////////////
    public class Expediente_unwind1
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public Cliente cliente { get; set; }
        public DateTime fechainicio { get; set; }
        public DateTime? fechafin { get; set; }
        public List<Derivation> derivaciones { get; set; } = new List<Derivation>();
        public string estado { get; set; }
        public DossierDocument documentos { get; set; } = new DossierDocument();
    }
    public class Expediente_lookup
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public Cliente cliente { get; set; }
        public DateTime fechainicio { get; set; }
        public DateTime? fechafin { get; set; }
        public List<Derivation> derivaciones { get; set; } = new List<Derivation>();
        public string estado { get; set; }
        public DossierDocument documentos { get; set; } = new DossierDocument();
        public List<Document> documentoobj { get; set; } = new List<Document>();
    }
    public class Expediente_unwind2
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public Cliente cliente { get; set; }
        public DateTime fechainicio { get; set; }
        public DateTime? fechafin { get; set; }
        public List<Derivation> derivaciones { get; set; } = new List<Derivation>();
        public string estado { get; set; }
        public DossierDocument documentos { get; set; } = new DossierDocument();
        public Document documentoobj { get; set; } = new Document();
    }

    public class Expediente_group
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string tipo { get; set; }
        public Cliente cliente { get; set; }
        public DateTime fechainicio { get; set; }
        public DateTime? fechafin { get; set; }
        public List<Derivation> derivaciones { get; set; } = new List<Derivation>();
        public string estado { get; set; }
        public List<DossierDocument> documentos { get; set; } = new List<DossierDocument>();
        public List<Document> documentoobj { get; set; } = new List<Document>();
    }
    ///ESTADISTICAS NUEVAS
    public class estadistica1_proyect1
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public int mes { get; set; }
        public string estado { get; set; }
        public string tipo { get; set; }
    }
    public class estadistica1_group
    {

        public string id { get; set; }
        public Int32 caducados { get; set; }
        public Int32 procesados { get; set; }
        public Int32 pendientes { get; set; }
    }
}
