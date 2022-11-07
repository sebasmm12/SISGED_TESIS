using Microsoft.JSInterop;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class SwalFireRepository : ISwalFireRepository
    {
        private readonly IJSRuntime _jsRuntime;

        public SwalFireRepository(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> ShowLockSwalFireAsync(SwalFireInfo swalFireInfo)
        {
            return await _jsRuntime.InvokeAsync<bool>("showLockSwalFire", swalFireInfo);
        }

        public async Task ShowSuccessfulSwalFireAsync(string htmlContent)
        {
            await ShowSwalFireAsync(new("Exitoso", htmlContent, SwalFireIcons.Success));
        }
        
        public async Task ShowErroSwalFireAsync(string htmlContent)
        {
            await ShowSwalFireAsync(new("Error", htmlContent, SwalFireIcons.Error));
        }

        public async Task ShowInfoSwalFireAsync(string htmlContent)
        {
            await ShowSwalFireAsync(new("Atención", htmlContent, SwalFireIcons.Info));
        }

        public async Task ShowWarningSwalFireAsync(string htmlContent)
        {
            await ShowSwalFireAsync(new("Cuidado", htmlContent, SwalFireIcons.Warning));
        }
        

        private async Task ShowSwalFireAsync(SwalFireInfo swalFireInfo)
        {
            await _jsRuntime.InvokeVoidAsync("showSwalFire", swalFireInfo);
        }
    }
}
