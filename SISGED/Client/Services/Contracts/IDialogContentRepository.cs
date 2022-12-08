using SISGED.Shared.Models.Requests.Documents;
using System;

namespace SISGED.Client.Services.Contracts
{
    public interface IDialogContentRepository
    {
        Task<T> ShowLoadingDialogAsync<T>(Func<Task<T>> action, string dialogTitle);
        Task<bool> ShowLoadingDialogAsync(Func<Task<bool>> action, string dialogTitle);
    }
}
