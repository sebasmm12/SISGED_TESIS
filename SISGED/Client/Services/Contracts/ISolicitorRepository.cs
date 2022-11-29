using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface ISolicitorRepository
    {
        Dictionary<string, object> ConvertToFilters(SolicitorFilter solicitorFilter);
    }
}
