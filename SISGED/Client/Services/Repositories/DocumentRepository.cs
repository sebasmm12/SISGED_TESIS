using SISGED.Client.Components.Documents.Informations;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private IDictionary<string, Type> _documentInfoComponents = new Dictionary<string, Type>()
        {
            { "SolicitudDenuncia", typeof(ComplaintRequestInfo) }
        };

        public IEnumerable<DocumentOption> GetDocumentTypesWithDossier()
        {
            return new List<DocumentOption>()
            {
                new("Solicitud de Búsqueda de Protocolo", "BPNRequest", Roles.MesaPartes),
                new("Oficio de Búsqueda de Protocolo", "OficioBPN", Roles.OrientacionLegal),
                new("Resultado de Búsqueda de Protocolo - Archivo General", "ResultadoArchivoBPN", Roles.ArchivosExnotarios),
                new("Pago de Protocolo Notarial", "PagoBPN", Roles.ArchivosExnotarios),
                new("Resultado de Búsqueda de Protocolo - MicroForma", "ResultadoMicroformaBPN", Roles.ArchivosExnotarios),
                new("Resultado de Búsqueda de Protocolo - Físico", "ResultadoProveedorBPN", Roles.ArchivosExnotarios),
                new("Solicitud de Expedición de Firma", "SignExpeditionRequest", Roles.MesaPartes),
                new("Oficio de Expedición de Firma", "OficioExpedicionFirma", Roles.OrientacionLegal),
                new("Oficio de Designación de Notario", "OficioDesignacionNotario", Roles.ArchivosExnotarios),
                new("Resolución de Sesión", "ResolucionSesion", Roles.SecretariaGeneral),
                new("Conclusión de Firma - Rechazo Notarial", "RechazoConclusionFirma", Roles.ArchivosExnotarios),
                new("Pago de Entrega de Testimonio", "PagoTestimonio", Roles.ArchivosExnotarios),
                new("Conclusión de Firma - Aprobación Notarial", "AprobacionConclusionFirma", Roles.ArchivosExnotarios),
                new("Solicitud de Denuncia", "ComplaintRequest", Roles.MesaPartes),
                new("Aperturamiento Disciplinario", "DisciplinaryOpennessRequest", Roles.TribunalHonor),
                new("Solicitud de Expediente de Notario", "SolicitorDossierRequest", Roles.Fiscal),
                new("Entrega de Expediente de Notario", "SolicitorDossierShipment", Roles.TramiteDocumentario), // To be implemented by Sebastian
                new("Dictamen", "Dictum", Roles.Fiscal),
                new("Resolución", "Resolution", Roles.TribunalHonor),
            };
        }

        public Type  GetDocumentInfoType(string documentType)
        {
            return _documentInfoComponents.FirstOrDefault(documentInfoComponent => documentInfoComponent.Key == documentType).Value;
        }
    }
}
