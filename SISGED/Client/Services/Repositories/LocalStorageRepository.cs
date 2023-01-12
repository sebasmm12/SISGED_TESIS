using Blazored.LocalStorage;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class LocalStorageRepository : ILocalStorageRepository
    {
        private readonly ILocalStorageService _localStorage;

        public LocalStorageRepository(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public async Task<string> GetFromLocalStorage(string key)
        {
            return await _localStorage.GetItemAsStringAsync(key);
        }

        public async Task RemoveItem(string key)
        {
            await _localStorage.RemoveItemAsync(key);
        }

        public async Task SetInLocalStorage(string key, string content)
        {
            await _localStorage.GetItemAsStringAsync(key, CancellationToken.None);
        }
    }
}
