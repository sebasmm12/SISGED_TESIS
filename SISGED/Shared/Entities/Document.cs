using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    [BsonDiscriminator("Documento")]
    [BsonKnownTypes(
        typeof(ComplaintRequest),
        typeof(BPNDocument),
        typeof(BPNRequest),
        typeof(BPNResult),
        typeof(SignExpeditionRequest),
        typeof(SolicitorDesignationDocument),
        typeof(SignConclusion),
        typeof(DisciplinaryOpenness),
        typeof(SolicitorDossierRequest),
        typeof(SolicitorDossierShipment),
        typeof(Dictum),
        typeof(Resolution),
        typeof(Appeal),
        typeof(InitialRequest))]
    public class Document
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("historialcontenido")]
        public List<ContentVersion> ContentsHistory { get; set; } = new();
        [BsonElement("historialproceso")]
        public List<Process> ProcessesHistory { get; set; } = new();
        [BsonElement("urlanexo")]
        public List<string> AttachedUrls { get; set; } = new();
        [BsonElement("estado")]
        public string State { get; set; } = default!;
        [BsonElement("fechacreacion")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow.AddHours(-5);

        public void AddProcess(Process process)
        {
            ProcessesHistory.Add(process);
        }
    }
    public class Evaluation
    {
        [BsonElement("resultado")]
        public string Result { get; set; } = default!;
        [BsonElement("evaluaciones")]
        public List<IndividualEvaluation> Evaluations { get; set; } = new();
    }

    public class IndividualEvaluation
    {
        [BsonElement("idparticipante")]
        public string ParticipantId { get; set; } = default!;
        [BsonElement("status")]
        public string Status { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
    }


    public class ComplaintRequestContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("idcliente")]
        public string ClientId { get; set; } = default!;
        [BsonElement("tipoDenuncia")]
        public string ComplaintType { get; set; } = default!;
        [BsonElement("fechaentrega")]
        public DateTime DeliveryDate { get; set; }
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;
    }

    [BsonDiscriminator("SolicitudDenuncia")]
    public class ComplaintRequest : Document
    {
        public ComplaintRequest(ComplaintRequestContent content, string state, List<string> urls)
        {
            Content = content;
            State = state;
            AttachedUrls = urls;
            Type = "SolicitudDenuncia";
            ContentsHistory = new();
            ProcessesHistory = new();

        }

        public ComplaintRequest() {  }

        [BsonElement("contenido")]
        public ComplaintRequestContent Content { get; set; } = new();
    }

    public class BPNDocumentContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("idcliente")]
        public string ClientId { get; set; } = default!;
        [BsonElement("direccionoficio")]
        public string DocumentAddress { get; set; } = default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("actojuridico")]
        public string JuridicalAct { get; set; } = default!;
        [BsonElement("tipoprotocolo")]
        public string ProtocolType { get; set; } = default!;
        [BsonElement("otorgantes")]
        public List<string> Grantors { get; set; } = new();
        [BsonElement("fecharealizacion")]
        public DateTime RealizationDate { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("OficioBPN")]
    public class BPNDocument : Document
    {
        [BsonElement("evaluacion")]
        public Evaluation Evaluation { get; set; } = default!;
        [BsonElement("contenido")]
        public BPNDocumentContent Content { get; set; } = new();

    }
    public class BPNRequestContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("idcliente")]
        public string ClientId { get; set; } = default!;
        [BsonElement("direccionoficio")]
        public string DocumentAddress { get; set; } = default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("actojuridico")]
        public string JuridicalAct { get; set; } = default!;
        [BsonElement("tipoprotocolo")]
        public string ProtocolType { get; set; } = default!;
        [BsonElement("otorgantes")]
        public List<string> Grantors { get; set; } = new();
        [BsonElement("fecharealizacion")]
        public DateTime RealizationDate { get; set; }
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("SolicitudBPN")]
    public class BPNRequest : Document
    {
        [BsonElement("contenido")]
        public BPNRequestContent Content { get; set; } = new();

    }
    public class BPNResultContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("costo")]
        public int Cost { get; set; }
        [BsonElement("cantidadfoja")]
        public int TotalSheets { get; set; }
        [BsonElement("estado")]
        public string Status { get; set; } = default!;
        [BsonElement("idescriturapublica")]
        public string PublicDeedId { get; set; } = default!;
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("ResultadoBPN")]

    public class BPNResult : Document
    {
        [BsonElement("contenido")]
        public BPNResultContent Content { get; set; } = default!;

    }

    public class SignExpeditionRequestContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("cliente")]
        public string Client { get; set; } = default!;
        [BsonElement("fecharealizacion")]
        public DateTime RealizationDate { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("SolicitudExpedicionFirma")]
    public class SignExpeditionRequest : Document
    {
        [BsonElement("contenido")]
        public SignExpeditionRequestContent Content { get; set; } = new();

    }
    public class SignConclusionContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("idescriturapublica")]
        public string PublicDeedId { get; set; } = default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("idcliente")]
        public string ClientId { get; set; } = default!;
        [BsonElement("cantidadfoja")]
        public int TotalSheets { get; set; }
        [BsonElement("precio")]
        public double Price { get; set; }
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("ConclusionFirma")]
    public class SignConclusion : Document
    {
        [BsonElement("contenido")]
        public SignConclusionContent Content { get; set; } = new();
    }
    
    [BsonDiscriminator("OficioDesignacionNotario")]
    public class SolicitorDesignationDocument : Document
    {
        [BsonElement("evaluacion")]
        public Evaluation Evaluation { get; set; } = new();
        [BsonElement("contenido")]
        public SolicitorDesignationDocumentContent Content { get; set; } = new();
    }
    public class SolicitorDesignationDocumentContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("fecharealizacion")]
        public DateTime RealizationDate { get; set; } = default!;
        [BsonElement("lugaroficionotarial")]
        public string SolicitorAddress { get; set; } = default!;
        [BsonElement("idusuario")]
        public string UserId { get; set; } = default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;
    }

    public class DisciplinaryOpennessContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("idfiscal")]
        public string ProsecutorId { get; set; } = default!;
        [BsonElement("nombredenunciante")] 
        public string ComplainantName { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("fechainicioaudiencia")]
        public DateTime AudienceStartDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("fechafinaudiencia")]
        public DateTime AudienceEndDate { get; set; } = DateTime.UtcNow.AddDays(-5);
        [BsonElement("participantes")]
        public List<string> Participants { get; set; } = new();
        [BsonElement("lugaraudiencia")]
        public string AudiencePlace { get; set; } = default!;
        [BsonElement("hechosimputados")]
        public List<string> ImputedFacts { get; set; } = new();
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;
        [BsonElement("iddenunciante")]
        public string ClientId { get; set; } = default!;
    }

    [BsonDiscriminator("AperturamientoDisciplinario")]
    public class DisciplinaryOpenness : Document
    {
        [BsonElement("contenido")]
        public DisciplinaryOpennessContent Content { get; set; } = default!;

        public DisciplinaryOpenness(DisciplinaryOpennessContent content, string state, List<string> urls)
        {
            Content = content;
            State = state;
            AttachedUrls = urls;
            Type = "AperturamientoDisciplinario";
            ContentsHistory = new();
            ProcessesHistory = new();

        }

        public DisciplinaryOpenness() { }
    }

    public class SolicitorDossierRequestContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("fechaemision")]
        public DateTime IssueDate { get; set; }
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;
        [BsonElement("iddenunciante")]
        public string ClientId { get; set; } = default!;
    }

    [BsonDiscriminator("SolicitudExpedienteNotario")]
    public class SolicitorDossierRequest : Document
    {
        [BsonElement("contenido")]
        public SolicitorDossierRequestContent Content { get; set; } = new();

        public SolicitorDossierRequest(SolicitorDossierRequestContent content, string state, List<string> urls)
        {
            Content = content;
            State = state;
            AttachedUrls = urls;
            Type = "SolicitudExpedienteNotario";
            ContentsHistory = new();
            ProcessesHistory = new();

        }

        public SolicitorDossierRequest() { }
    }

    public class DictumContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("iddenunciante")]
        public string ComplaintId { get; set; }=  default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("observaciones")]
        public List<string> Observations { get; set; } = new();
        [BsonElement("conclusion")]
        public string Conclusion { get; set; } = default!;
        [BsonElement("recomendaciones")]
        public List<string> Recommendations { get; set; } = new();
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;
    }

    [BsonDiscriminator("Dictamen")]
    public class Dictum : Document
    {
        public Dictum(DictumContent content, string state, List<string> urls)
        {
            Content = content;
            State = state;
            AttachedUrls = urls;
            Type = "Dictamen";
            ContentsHistory = new();
            ProcessesHistory = new();

        }

        public Dictum() { }

        [BsonElement("contenido")]
        public DictumContent Content { get; set; } = new();
    }

    public class ResolutionContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("fechainicioaudiencia")]
        public DateTime AudienceStartDate { get; set; }
        [BsonElement("fechafinaudiencia")]
        public DateTime AudienceEndDate { get; set; }
        [BsonElement("participantes")]
        public List<string> Participants { get; set; } = new();
        [BsonElement("sancion")]
        public string Sanction { get; set; } = default!;
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;
        [BsonElement("iddenunciante")]
        public string ClientId { get; set; } = default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
    }

    [BsonDiscriminator("Resolucion")]
    public class Resolution : Document
    {
        [BsonElement("evaluacion")]
        public Evaluation Evaluation { get; set; } = default!;
        [BsonElement("contenido")]
        public ResolutionContent Content { get; set; } = default!;

        public Resolution(ResolutionContent content, string state, List<string> urls)
        {
            Content = content;
            State = state;
            AttachedUrls = urls;
            Type = "Resolucion";
            ContentsHistory = new();
            ProcessesHistory = new();

        }

        public Resolution() { }
    }
    public class AppealContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("fechaapelacion")]
        public DateTime AppealDate { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("Apelacion")]
    public class Appeal : Document
    {
        [BsonElement("evaluacion")]
        public Evaluation Evaluation { get; set; } = new();
        [BsonElement("contenido")]
        public AppealContent Content { get; set; } = new();
    }

    public class InitialRequestContent
    {
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("idtiposolicitud")]
        public string RequestTypeId { get; set; } = default!;
        [BsonElement("tienenotario")]
        public bool HasSolicitor { get; set; }
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;

    }

    [BsonDiscriminator("SolicitudInicial")]
    public class InitialRequest : Document
    {
        public InitialRequest() {  }

        public InitialRequest(InitialRequestContent content, string state, List<string> urls)
        {
            Content = content;
            State = state;
            Type = "SolicitudInicial";
            ContentsHistory = new();
            ProcessesHistory = new();
            AttachedUrls = urls;
        }

        [BsonElement("contenido")]
        public InitialRequestContent Content { get; set; } = new();
    }

    public class SolicitorDossierShipmentContent
    {
        [BsonElement("codigo")]
        public string Code { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("expedientes")]
        public List<string>? SolicitorDossiers { get; set; }
        [BsonElement("firma")]
        public string Sign { get; set; } = default!;
        [BsonElement("urlGenerado")]        
        public string GeneratedUrl { get; set; } = default!;
    }

    [BsonDiscriminator("EntregaExpedienteNotario")]
    public class SolicitorDossierShipment : Document
    {
        public SolicitorDossierShipment() { }

        public SolicitorDossierShipment(SolicitorDossierShipmentContent content, string state, List<string> urls)
        {
            Content = content;
            State = state;
            Type = "EntregaExpedienteNotario";
            ContentsHistory = new();
            ProcessesHistory = new();
            AttachedUrls = urls;
        }

        [BsonElement("contenido")]
        public SolicitorDossierShipmentContent Content { get; set; } = new();
    }
}
