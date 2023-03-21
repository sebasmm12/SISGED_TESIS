namespace SISGED.Client.Helpers
{
    public class AsistantMessageUpdate
    {
        public AsistantMessageUpdate(string dossierType, string documentType, int subStep)
        {
            DossierType = dossierType;
            DocumentType = documentType;
            SubStep = subStep;
        }

        public string DossierType { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public int SubStep { get; set; } = default!;
    }
}
