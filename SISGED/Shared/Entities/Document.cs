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
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("contentsHistory")]
        public List<ContentVersion> ContentsHistory { get; set; } = new();
        [BsonElement("processesHistory")]
        public List<Process> ProcessesHistory { get; set; } = new();
        [BsonElement("evaluations")]
        public List<DocumentEvaluation> Evaluations { get; set; } = new();
        [BsonElement("attachedUrls")]
        public List<string> AttachedUrls { get; set; } = new();
        [BsonElement("state")]
        public string State { get; set; } = default!;
        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow.AddHours(-5);

        [BsonIgnore]
        private readonly IEnumerable<string> derivatedStates = new List<string> { "derivado" };

        public void AddProcess(Process process)
        {
            ProcessesHistory.Add(process);
        }

        public bool IsDerivated()
        {
            return derivatedStates.Contains(State);
        }

        public bool IsTypeOf(string documentType)
        {
            return Type == documentType;
        }

        public Process GetLastProcess()
        {
            return ProcessesHistory.Last();
        }
    }
    public class Evaluation
    {
        [BsonElement("result")]
        public string Result { get; set; } = default!;
        [BsonElement("evaluations")]
        public List<IndividualEvaluation> Evaluations { get; set; } = new();
    }

    public class IndividualEvaluation
    {
        [BsonElement("participantId")]
        public string ParticipantId { get; set; } = default!;
        [BsonElement("status")]
        public string Status { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
    }


    public class ComplaintRequestContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("clientId")]
        public string ClientId { get; set; } = default!;
        [BsonElement("complaintType")]
        public string ComplaintType { get; set; } = default!;
        [BsonElement("deliveryDate")]
        public DateTime DeliveryDate { get; set; }
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
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

        public ComplaintRequest() { }

        [BsonElement("content")]
        public ComplaintRequestContent Content { get; set; } = new();
    }

    public class BPNDocumentContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("clientId")]
        public string ClientId { get; set; } = default!;
        [BsonElement("documentAddress")]
        public string DocumentAddress { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("juridicalAct")]
        public string JuridicalAct { get; set; } = default!;
        [BsonElement("protocolType")]
        public string ProtocolType { get; set; } = default!;
        [BsonElement("grantors")]
        public List<string> Grantors { get; set; } = new();
        [BsonElement("realizationDate")]
        public DateTime RealizationDate { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("OficioBPN")]
    public class BPNDocument : Document
    {
        [BsonElement("evaluation")]
        public Evaluation Evaluation { get; set; } = default!;
        [BsonElement("content")]
        public BPNDocumentContent Content { get; set; } = new();

    }
    public class BPNRequestContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("clientId")]
        public string ClientId { get; set; } = default!;
        [BsonElement("documentAddress")]
        public string DocumentAddress { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("juridicalAct")]
        public string JuridicalAct { get; set; } = default!;
        [BsonElement("protocolType")]
        public string ProtocolType { get; set; } = default!;
        [BsonElement("grantors")]
        public List<string> Grantors { get; set; } = new();
        [BsonElement("realizationDate")]
        public DateTime RealizationDate { get; set; }
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("SolicitudBPN")]
    public class BPNRequest : Document
    {
        [BsonElement("content")]
        public BPNRequestContent Content { get; set; } = new();

    }
    public class BPNResultContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("cost")]
        public int Cost { get; set; }
        [BsonElement("totalSheets")]
        public int TotalSheets { get; set; }
        [BsonElement("status")]
        public string Status { get; set; } = default!;
        [BsonElement("publicDeedId")]
        public string PublicDeedId { get; set; } = default!;
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("ResultadoBPN")]

    public class BPNResult : Document
    {
        [BsonElement("content")]
        public BPNResultContent Content { get; set; } = default!;

    }

    public class SignExpeditionRequestContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("client")]
        public string Client { get; set; } = default!;
        [BsonElement("realizationDate")]
        public DateTime RealizationDate { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("SolicitudExpedicionsign")]
    public class SignExpeditionRequest : Document
    {
        [BsonElement("content")]
        public SignExpeditionRequestContent Content { get; set; } = new();

    }
    public class SignConclusionContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("publicDeedId")]
        public string PublicDeedId { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("clientId")]
        public string ClientId { get; set; } = default!;
        [BsonElement("totalSheets")]
        public int TotalSheets { get; set; }
        [BsonElement("price")]
        public double Price { get; set; }
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("Conclusionsign")]
    public class SignConclusion : Document
    {
        [BsonElement("content")]
        public SignConclusionContent Content { get; set; } = new();
    }

    [BsonDiscriminator("OficioDesignacionNotario")]
    public class SolicitorDesignationDocument : Document
    {
        [BsonElement("evaluation")]
        public Evaluation Evaluation { get; set; } = new();
        [BsonElement("content")]
        public SolicitorDesignationDocumentContent Content { get; set; } = new();
    }
    public class SolicitorDesignationDocumentContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("realizationDate")]
        public DateTime RealizationDate { get; set; } = default!;
        [BsonElement("solicitorAddress")]
        public string SolicitorAddress { get; set; } = default!;
        [BsonElement("userId")]
        public string UserId { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;
    }

    public class DisciplinaryOpennessContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("audienceStartDate")]
        public DateTime AudienceStartDate { get; set; } = default!;
        [BsonElement("audienceEndDate")]
        public DateTime AudienceEndDate { get; set; } = default!;
        [BsonElement("participants")]
        public List<string> Participants { get; set; } = new();
        [BsonElement("audiencePlace")]
        public string AudiencePlace { get; set; } = default!;
        [BsonElement("imputedFacts")]
        public List<string> ImputedFacts { get; set; } = new();
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;
        [BsonElement("clientId")]
        public string ClientId { get; set; } = default!;
    }

    [BsonDiscriminator("AperturamientoDisciplinario")]
    public class DisciplinaryOpenness : Document
    {
        [BsonElement("content")]
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
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("issueDate")]
        public DateTime IssueDate { get; set; }
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;
        [BsonElement("clientId")]
        public string ClientId { get; set; } = default!;
    }

    [BsonDiscriminator("SolicitudExpedienteNotario")]
    public class SolicitorDossierRequest : Document
    {
        [BsonElement("content")]
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
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("complaintId")]
        public string ComplaintId { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("observations")]
        public List<string> Observations { get; set; } = new();
        [BsonElement("conclusion")]
        public string Conclusion { get; set; } = default!;
        [BsonElement("recommendations")]
        public List<string> Recommendations { get; set; } = new();
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
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

        [BsonElement("content")]
        public DictumContent Content { get; set; } = new();
    }

    public class ResolutionContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("audienceStartDate")]
        public DateTime AudienceStartDate { get; set; }
        [BsonElement("audienceEndDate")]
        public DateTime AudienceEndDate { get; set; }
        [BsonElement("participants")]
        public List<string> Participants { get; set; } = new();
        [BsonElement("sanction")]
        public string Sanction { get; set; } = default!;
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;
        [BsonElement("clientId")]
        public string ClientId { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
    }

    [BsonDiscriminator("Resolucion")]
    public class Resolution : Document
    {
        [BsonElement("content")]
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
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("appealDate")]
        public DateTime AppealDate { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; } = default!;

    }

    [BsonDiscriminator("Apelacion")]
    public class Appeal : Document
    {
        [BsonElement("evaluation")]
        public Evaluation Evaluation { get; set; } = new();
        [BsonElement("content")]
        public AppealContent Content { get; set; } = new();
    }

    public class InitialRequestContent
    {
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("requestTypeId")]
        public string RequestTypeId { get; set; } = default!;
        [BsonElement("hasSolicitor")]
        public bool HasSolicitor { get; set; }
        [BsonElement("solicitorId")]
        public string? SolicitorId { get; set; }

    }

    [BsonDiscriminator("SolicitudInicial")]
    public class InitialRequest : Document
    {
        public InitialRequest() { }

        public InitialRequest(InitialRequestContent content, string state, List<string> urls)
        {
            Content = content;
            State = state;
            Type = "SolicitudInicial";
            ContentsHistory = new();
            ProcessesHistory = new();
            AttachedUrls = urls;
        }

        [BsonElement("content")]
        public InitialRequestContent Content { get; set; } = new();
    }

    public class SolicitorDossierShipmentContent
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("solicitorDossiers")]
        public List<string>? SolicitorDossiers { get; set; }
        [BsonElement("sign")]
        public string Sign { get; set; } = default!;
        [BsonElement("generatedUrl")]
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

        [BsonElement("content")]
        public SolicitorDossierShipmentContent Content { get; set; } = new();
    }
}
