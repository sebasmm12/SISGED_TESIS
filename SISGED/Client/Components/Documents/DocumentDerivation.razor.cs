using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Client.Components.Documents
{
    public partial class DocumentDerivation
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;

        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;
        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private bool pageLoading = true;
        private Role? userRole = default!;
        private readonly string currentDate = DateTime.UtcNow.AddHours(-5).ToString("dd/MM/yyyy");
        // TODO: Get the roleId based on the derivation step from the helper
        private string roleId = "5eeaf8d58ca4ff53a0b791ea";
        private Role? receiverUserRole = default!;
        private List<UserTrayResponse> userTrays = default!;
        private UserTrayResponse userTray = default!;


        protected override async Task OnInitializedAsync()
        {
            await GetUserInformationAsync();

            userTrays = await GetUserTraysAsync(receiverUserRole!.Name);

            pageLoading = false;
        }

        private async Task GetUserInformationAsync()
        {
            var userRoleTask =  GetRoleAsync(SessionAccount.GetUser().Rol);
            var receiverUserRoleTask =  GetRoleAsync(roleId);

            await Task.WhenAll(userRoleTask, receiverUserRoleTask);

            userRole = await userRoleTask;
            receiverUserRole = await receiverUserRoleTask;
        }


        private async Task<Role?> GetRoleAsync(string roleId)
        {
            try
            {
                var roleResponse = await HttpRepository.GetAsync<Role>($"api/accounts/roles/{roleId}");

                if(roleResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo obtener información sobre su rol");
                }

                return roleResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo obtener información sobre su rol");
                return null;
            }
        }

        private async Task<List<UserTrayResponse>> GetUserTraysAsync(string trayType)
        {
            // TODO: Implement the endpoint to get the sorted user trays

            try
            {
                var userTraysResponse = await HttpRepository.GetAsync<List<UserTrayResponse>>($"api/trays/{trayType}");

                if (userTraysResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo obtener información de los usuarios para la derivación");
                }

                return userTraysResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync($"No se pudo obtener información de los usuarios para la derivación");
                return new();
            }
        }
    }
}