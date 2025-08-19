namespace SISGED.Client.Helpers;

public static class DossierTypeMapper
{
    public static string Map(string dossierType)
    {
        return dossierType switch
        {
            "Solicitud" => "Solicitud",
            "Denuncia" => "Procedimiento Disciplinario",
            _ => dossierType
        };
    }
}