using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class AnnexFactory : IAnnexFactory
    {
        private readonly IEnumerable<AnnexPreview> Annexes = new List<AnnexPreview>()
        {
            new("fa-file-pdf", "mud-error-text", new List<string>() { ".pdf" }),
            new("fa-file-excel", "mud-success-text", new List<string>() { ".xls", ".xlsx" }),
            new("fa-file-word", "mud-info-text", new List<string>() { ".doc", ".docx" }),
            new("fa-file-image", "mud-warning-text", new List<string>() { ".jpg", ".png", ".jpeg", ".gif", ".bmp", ".webp" })

        };

        public AnnexPreview GetAnnexPreview(string extension)
        {
            var annexPreview = Annexes.FirstOrDefault(annex => annex.Extensions.Contains(extension), new("fa-file", ""));

            if (annexPreview is null) throw new Exception($"No se pudo encontrar la previsualización del anexo con extensión { extension }");

            return annexPreview;
        }
    }
}
