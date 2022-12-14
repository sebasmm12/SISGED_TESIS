using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    //public class ExpedienteDTO
    //{
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public List<DossierDocument> documentos { get; set; } = new List<DossierDocument>();
    //    public List<Document> documentosobj { get; set; } = new List<Document>();
    //    public List<Derivation> derivaciones { get; set; } = new List<Derivation>();
    //    public string estado { get; set; }
    //}

    //public class ExpedienteDocumentoDTO
    //{
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; }
    //    public List<Derivation> derivaciones { get; set; } = new List<Derivation>();
    //    public string estado { get; set; }
    //}

    //public class ExpedienteDTO2
    //{
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public List<DossierDocument> documentos { get; set; } = new List<DossierDocument>();
    //    public List<Document> documentosobj { get; set; } = new List<Document>();
    //    public List<Derivation> derivaciones { get; set; } = new List<Derivation>();
    //    public string estado { get; set; }
    //}


    //public class ExpedienteDTO_ur1
    //{
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; }
    //    public List<Derivation> derivaciones { get; set; }
    //    public string estado { get; set; }
    //}
    //public class ExpedienteDTO_look_up
    //{
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; }
    //    public List<Derivation> derivaciones { get; set; }
    //    public string estado { get; set; }
    //    public List<Document> documentoobj { get; set; }
    //}
    //public class ExpedienteDTO_ur2
    //{
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; }
    //    public List<Derivation> derivaciones { get; set; }
    //    public string estado { get; set; }
    //    public Document documentoobj { get; set; }
    //}
    //public class ExpedienteWrapper
    //{
    //    public string idexpediente { get; set; }
    //    public object documento { get; set; }
    //    public string idusuarioactual { get; set; }
    //    public string documentoentrada { get; set; }
    //}

    //public class ExpedienteFinal
    //{
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public Document documento { get; set; } = new Document();
    //    public List<Document> documentosobj { get; set; } = new List<Document>();
    //}

    //public class Expedientedoc
    //{

    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; } = new DossierDocument();
    //    public List<Derivation> derivaciones { get; set; }
    //    public string estado { get; set; }
    //}

    //public class Expedientedoc_lookup
    //{

    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; } = new DossierDocument();
    //    public List<Derivation> derivaciones { get; set; }
    //    public List<DocumentoDTO> documentosobj { get; set; } = new List<DocumentoDTO>();
    //    public string estado { get; set; }
    //}

    //public class Expedientedoc_lookup_ur
    //{

    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; } = new DossierDocument();
    //    public List<Derivation> derivaciones { get; set; }
    //    public DocumentoDTO documentosobj { get; set; } = new DocumentoDTO();
    //    public string estado { get; set; }
    //}

    //public class Expedientedoc_group
    //{

    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public List<DocumentoDTO> documentosobj { get; set; } = new List<DocumentoDTO>();

    //}

    //public class ExpedienteDocumentoBPNDTO
    //{
    //    public Dossier expediente { get; set; }
    //    public SolicitudBPN solicitduBPN { get; set; }
    //}

    //public class ExpedienteDocumentoEFDTO
    //{
    //    public Dossier expediente { get; set; }
    //    public SolicitudExpedicionFirma solicitudEF { get; set; }
    //}

    //public class ExpedienteDocumentoSDDTO
    //{
    //    public Dossier expediente { get; set; }
    //    public SolicitudDenuncia solicitudD { get; set; }
    //}
    //public class ExpedienteDocumentoSIDTO
    //{
    //    public Dossier expediente { get; set; }
    //    public SolicitudInicial solicitudI { get; set; }
    //}
    ////Clases DTO del diagrama GANTT
    //public class ExpedineteDTO_unwind1
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; } = new DossierDocument();
    //    public List<Derivation> derivaciones { get; set; }
    //    public string estado { get; set; }
    //}
    //public class ExpedineteDTO_lookup1g
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; } = new DossierDocument();
    //    public List<Derivation> derivaciones { get; set; }
    //    public List<Document> documentoobj { get; set; }
    //    public string estado { get; set; }
    //}
    //public class ExpedineteDTO_unwind2g
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; } = new DossierDocument();
    //    public List<Derivation> derivaciones { get; set; }
    //    public Document documentoobj { get; set; }
    //    public string estado { get; set; }
    //}
    //public class ExpedienteDTO_lookup
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; } = new DossierDocument();
    //    public List<Derivation> derivaciones { get; set; }
    //    public List<DocumentoDTO> documentosobj { get; set; } = new List<DocumentoDTO>();
    //    public string estado { get; set; }
    //}
    //public class ExpedienteDTO_unwind2
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public DossierDocument documentos { get; set; } = new DossierDocument();
    //    public List<Derivation> derivaciones { get; set; }
    //    public DocumentoDTO documentosobj { get; set; } = new DocumentoDTO();
    //    public string estado { get; set; }
    //}

    //public class ExpedienteDTO_group1
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipo { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechainicio { get; set; }
    //    public DateTime? fechafin { get; set; }
    //    public List<DossierDocument> documentos { get; set; } = new List<DossierDocument>();
    //    public List<Derivation> derivaciones { get; set; }
    //    public List<DocumentoDTO> documentosobj { get; set; } = new List<DocumentoDTO>();
    //    public string estado { get; set; }
    //}

    ////objetos gantt 2

    //public class expediente_project
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string id { get; set; }
    //    public string tipoexpediente { get; set; }
    //    public string iddocumento { get; set; }
    //    public string tipodocumento { get; set; }
    //    public Cliente cliente { get; set; }
    //    public DateTime fechacreacion { get; set; }
    //    public DateTime? fechademora { get; set; }
    //    public DateTime fechaexceso { get; set; }
    //    public string estado { get; set; }
    //}
}
