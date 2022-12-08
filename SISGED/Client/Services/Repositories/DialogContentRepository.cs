using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Generics;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Requests.Documents;

namespace SISGED.Client.Services.Repositories
{
    public class DialogContentRepository : IDialogContentRepository
    {

        private readonly IDialogService _dialogService;

        public DialogContentRepository(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<bool> ShowLoadingDialogAsync(Func<Task<bool>> action, string dialogTitle)
        {
            var dialogParameters = GetDialogParameters(new()
            {
               new("Body", "Realizando la operación, por favor espere..."),
               new("DialogMethod", action.Invoke())
            });

            var dialog = await InvokeDialogAsync<GenericDialogContent<bool>>(dialogTitle, dialogParameters);

            if (dialog.Cancelled) return false;

            _ = bool.TryParse(dialog.Data.ToString(), out bool isChanged);

            return isChanged;
        }



        private static DialogParameters GetDialogParameters(List<DialogParameter> dialogParameterDTOs)
        {
            var dialogParameters = new DialogParameters();

            dialogParameterDTOs.ForEach(dialogParameterDTO =>
            {
                dialogParameters.Add(dialogParameterDTO.Name, dialogParameterDTO.Value);
            });

            return dialogParameters;
        }

        private async Task<DialogResult> InvokeDialogAsync<T>(string title, DialogParameters dialogParameters) where T : ComponentBase
        {
            var dialogService = _dialogService.Show<T>(title, dialogParameters);
            return await dialogService.Result;
        }

        public async Task<T> ShowLoadingDialogAsync<T>(Func<Task<T>> action, string dialogTitle)
        {
            var dialogParameters = GetDialogParameters(new()
            {
               new("Body", "Realizando la operación, por favor espere..."),
               new("DialogMethod", action.Invoke())
            });

            var dialog = await InvokeDialogAsync<GenericDialogContent<T>>(dialogTitle, dialogParameters);

            if (dialog.Cancelled) return default!;

            return (T)dialog.Data;
        }
    }
}
