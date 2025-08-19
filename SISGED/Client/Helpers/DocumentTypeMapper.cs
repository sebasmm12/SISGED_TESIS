namespace SISGED.Client.Helpers;

public static class DocumentTypeMapper
{
    public static string Map(string documentType)
    {
        return documentType switch
        {
            "SolicitudInicial" => "Solicitud Inicial",
            "SolicitudDenuncia" => "Solicitud de Denuncia",
            "AperturamientoDisciplinario" => "Aperturamiento Disciplinario",
            "SolicitudExpedienteNotario" => "Solicitud de Expediente del notario",
            "EntregaExpedienteNotario" => "Entrega de Expediente del notario",
            "Dictamen" => "Dictamen",
            "Resolucion" => "Resolución",
            "ResolucionSesion" => "Resolución de la sesión",
            _ => documentType
        };
    }
}