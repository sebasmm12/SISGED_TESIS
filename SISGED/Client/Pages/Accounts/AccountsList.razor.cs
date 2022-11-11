using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Generics;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.User;
using System.Linq.Expressions;

namespace SISGED.Client.Pages.Accounts
{
    public partial class AccountsList
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        private bool usersLoading = true;
        private MudTable<UserInfoResponse> usersList = default!;

        private int TotalUsers => (usersList.GetFilteredItemsCount() + usersList.RowsPerPage - 1) / usersList.RowsPerPage;

        private static string GetLockedUserText(string userState)
        {
            return userState == "activo" ? "Bloquear" : "Desbloquear";
        }

        private static Color GetLockedUserColor(string userState)
        {
            return userState == "activo" ? Color.Error : Color.Success;
        }

        private static Color GetUserStateColor(string userState)
        {
            return userState == "activo" ? Color.Success : Color.Error;
        }

        private void ChangePage(int page)
        {
            usersList.NavigateTo(page - 1);
        }

        private async Task<TableData<UserInfoResponse>> ReloadTableAsync(TableState tableState)
        {
            var users = await GetUsersAsync(tableState);

            await Task.Delay(100);

            return new TableData<UserInfoResponse>() { Items = users.Users, TotalItems = (int)users.TotalUsers };
        }

        private async Task<PaginatedUserInfoResponse> GetUsersAsync(TableState tableState)
        {
            try
            {
                string userQueries = GetQueriesFromTableState(tableState);


                var usersResponse = await HttpRepository.GetAsync<PaginatedUserInfoResponse>($"api/users{userQueries}");

                if (usersResponse.Error)
                {
                    await SwalFireRepository.ShowErroSwalFireAsync("No se pudo obtener los usuarios del sistema");
                }
                
                if(usersLoading) usersLoading = false;

                return usersResponse.Response!;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErroSwalFireAsync("No se pudo obtener los usuarios del sistema");
                return new PaginatedUserInfoResponse(new List<UserInfoResponse>(), 0);
            }
        }

        private static string GetQueriesFromTableState(TableState tableState)
        {
            string userQueries = "?";

            userQueries += $"page={System.Web.HttpUtility.UrlEncode(tableState.Page.ToString())}";
            userQueries += $"&pagesize={System.Web.HttpUtility.UrlEncode(tableState.PageSize.ToString())}";

            return userQueries;

        }
        
        private async Task ChangeUserStateAsync(UserInfoResponse userInfoResponse)
        {
            var swalFireInfo = GetSwalFireInfo(userInfoResponse);

            bool userChanged = await SwalFireRepository.ShowLockSwalFireAsync(swalFireInfo);
            
            if (!userChanged) return;
            
            string userStateMessage = userInfoResponse.State == "activo" ? "bloquear" : "desbloquear";

            bool isChanged = await ShowLoadingDialogAsync(userInfoResponse, userStateMessage);

            if (!isChanged) return;

            await SwalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo {userStateMessage} " +
                    $"la cuenta del usuario {userInfoResponse.UserName} satisfactoriamente");

            await usersList.ReloadServerData();
        }

        private async Task<bool> ShowLoadingDialogAsync(UserInfoResponse userInfoResponse, string userStateMessage)
        {
           string dialogTitle = $"Cambiando estado de la cuenta del usuario {userInfoResponse.UserName}";

           var dialogParameters = GetDialogParameters(new()
           {
               new("Body", "Realizando la operación, por favor espere..."),
               new("DialogMethod", UpdateUserStateAsync(userInfoResponse, userStateMessage))
           });

           var dialog = await InvokeDialogAsync<GenericDialogContent>(dialogTitle, dialogParameters);

           if(dialog.Cancelled) return false;

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
            var dialogService = DialogService.Show<T>(title, dialogParameters);
            return await dialogService.Result;
        }

        private static SwalFireInfo GetSwalFireInfo(UserInfoResponse userInfoResponse)
        {
            string action = userInfoResponse.State == "activo" ? "bloquear" : "desbloquear";
            string actionTitle = userInfoResponse.State == "activo" ? "Bloqueo" : "Desbloqueo";

            string title = $"{actionTitle} del usuario {userInfoResponse.UserName}";
            string htmlContent = $"¿Está seguro que desea {action} al usuario {userInfoResponse.UserName}?";
            
            return new SwalFireInfo(title, htmlContent, SwalFireIcons.Warning, action);
        }

        public async Task<bool> UpdateUserStateAsync(UserInfoResponse user, string userStateMessage)
        {
            bool isChanged = true;

            try
            {
                var deletedUserResponse = await HttpRepository.DeleteAsync($"api/users/{user.Id}");

                if (deletedUserResponse.Error)
                {
                    await SwalFireRepository.ShowErroSwalFireAsync($"No se pudo {userStateMessage} al usuario {user.UserName}");
                    isChanged = false;
                }
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErroSwalFireAsync($"No se pudo {userStateMessage} al usuario {user.UserName}");
                isChanged = false;
            }

            return isChanged;
        }
    }
}
