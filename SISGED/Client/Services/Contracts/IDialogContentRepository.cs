using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IDialogContentRepository
    {
        Task ShowDialogAsync<T>(List<DialogParameter> dialogParameters, string dialogTitle) where T : ComponentBase;
        Task ShowDialogAsync(Type component, List<DialogParameter> dialogParameters, string dialogTitle);
        Task<T> ShowLoadingDialogAsync<T>(Func<Task<T>> action, string dialogTitle);
        Task<bool> ShowLoadingDialogAsync(Func<Task<bool>> action, string dialogTitle);
    }
}
