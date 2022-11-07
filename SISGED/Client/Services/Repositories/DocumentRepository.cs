using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        public IEnumerable<DocumentOption> GetDocumentTypesWithDossier()
        {
            return new List<DocumentOption>()
            {
                new("Documento de Solicitud de Búsqueda de Protocolo", "SolicitudBPN", Roles.MesaPartes),
                new("Documento de Oficio de Búsqueda de Protocolo", "OficioBPN", Roles.OrientacionLegal),
                new("Documento de Resultado de Búsqueda de Protocolo", "ResultadoBPN", Roles.ArchivosExnotarios),
                new("Documento de Solicitud de Expedición de Firma", "SolicitudExpedicionFirma", Roles.MesaPartes),
                new("Documento de Oficio de Designación de Notario", "OficioDesignacionNotario", Roles.ArchivosExnotarios),
                new("Documento de Conclusión de Firma", "ConclusionFirma", Roles.ArchivosExnotarios),
                new("Documento de Solicitud de Denuncia", "SolicitudDenuncia", Roles.MesaPartes),
                new("Documento de Aperturamiento Disciplinario", "AperturamientoDisciplinario", Roles.TribunalHonor),
                new("Documento de Solicitud de Expediente de Notario", "SolicitudExpedienteNotario", Roles.TribunalHonor),
                
            };
        }

        public IEnumerable<DocumentOption> GetDocumentTypesWithOutDossier()
        {
            throw new NotImplementedException();
        }
    }
}
