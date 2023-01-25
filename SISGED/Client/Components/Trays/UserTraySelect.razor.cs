using Microsoft.AspNetCore.Components;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Responses.Tray;
using System.Linq.Expressions;

namespace SISGED.Client.Components.Trays
{
    public partial class UserTraySelect
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [Parameter]
        public string RoleId { get; set; } = default!;
        [Parameter]
        public Expression<Func<UserTrayResponse>> UserTrayFor { get; set; } = default!;
        [Parameter]
        public EventCallback<UserTrayResponse> UserTrayUpdate { get; set; } = default!;

        private List<UserTrayResponse> userTrays = default!;
        private bool pageLoading = true;

        protected async override Task OnInitializedAsync()
        {
            userTrays = await GetUserTraysAsync(RoleId);

            pageLoading = false;
        }

        private async void UpdateUserTray(UserTrayResponse userTrayResponse)
        {
            await UserTrayUpdate.InvokeAsync(userTrayResponse);
        }

        private async Task<List<UserTrayResponse>> GetUserTraysAsync(string roleId)
        {
            try
            {
                var userTraysResponse = await HttpRepository.GetAsync<List<UserTrayResponse>>($"api/tray/workloadbyrole/{roleId}");

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