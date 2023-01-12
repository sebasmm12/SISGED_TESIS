namespace SISGED.Client.Services.Contracts
{
    public interface ILocalStorageRepository
    {
        Task SetInLocalStorage(string key, string content);
        Task<string> GetFromLocalStorage(string key);
        Task RemoveItem(string key);
    }
}
