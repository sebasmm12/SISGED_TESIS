using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IAnnexFactory
    {
        AnnexPreview GetAnnexPreview(string extension);
    }
}
