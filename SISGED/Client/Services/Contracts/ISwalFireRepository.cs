using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface ISwalFireRepository
    {
        Task ShowErroSwalFireAsync(string htmlContent);
        Task ShowInfoSwalFireAsync(string htmlContent);
        Task<bool> ShowLockSwalFireAsync(SwalFireInfo swalFireInfo);
        Task ShowSuccessfulSwalFireAsync(string htmlContent);
        Task ShowWarningSwalFireAsync(string htmlContent);
    }
}
