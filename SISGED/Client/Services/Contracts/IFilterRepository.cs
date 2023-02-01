using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IFilterRepository<T>
    {
        Dictionary<string, object> ConvertToFilters(T solicitorFilter);
    }
}
