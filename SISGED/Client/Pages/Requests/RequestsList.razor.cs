using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.User;

namespace SISGED.Client.Pages.Requests
{
    public partial class RequestsList
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        
        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private bool requestsLoading = true;
        private MudTable<UserRequestWithPublicDeedResponse> requestsList = default!;

        // TODO: Get the information based on the session and not with this value
        private readonly string documentNumber = "70477724";


        private int TotalUserRequests => (requestsList.GetFilteredItemsCount() + requestsList.RowsPerPage - 1) / requestsList.RowsPerPage;

        private async Task CreateUserRequest()
        {
            var dialogParameters = GetDialogParameters(new()
            {
                new("SessionAccount", SessionAccount)
            });

            var dialog = DialogService.Show<UserRequestRegister>("Registro de Solicitud", dialogParameters,
                new DialogOptions() { FullWidth = true, MaxWidth = MaxWidth.Large, Position = DialogPosition.Center, NoHeader = true});

            var dialogResult = await dialog.Result;

            if (dialogResult.Cancelled) return;

            await requestsList.ReloadServerData();
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

        private void ChangePage(int page)
        {
            requestsList.NavigateTo(page - 1);
        }

        private async Task<TableData<UserRequestWithPublicDeedResponse>> ReloadTableAsync(TableState tableState)
        {
            var userRequests = await GetUserRequestsAsync(tableState);

            await Task.Delay(100);

            return new TableData<UserRequestWithPublicDeedResponse>() { Items = userRequests.UserRequests, 
                TotalItems = (int)userRequests.TotalUserRequests };
        }


        private async Task<PaginatedUserRequest> GetUserRequestsAsync(TableState tableState)
        {
            try
            {
                string userRequestQueries = GetQueriesForUserRequests(tableState);

                var userRequestsResponse = await HttpRepository.GetAsync<PaginatedUserRequest>($"api/documents/user-requests-public-deeds{userRequestQueries}");

                if(userRequestsResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener las solicitudes del sistema");
                }

                if (requestsLoading) requestsLoading = false;

                return userRequestsResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener sus solicitudes registradas en el sistema");
                return new PaginatedUserRequest(new List<UserRequestWithPublicDeedResponse>(), 0);
            }
        }

        private string GetQueriesForUserRequests(TableState tableState)
        {
            string userRequestQueries = "?";

            userRequestQueries += $"page={System.Web.HttpUtility.UrlEncode(tableState.Page.ToString())}";
            userRequestQueries += $"&pagesize={System.Web.HttpUtility.UrlEncode(tableState.PageSize.ToString())}";
            userRequestQueries += $"&documentNumber={System.Web.HttpUtility.UrlEncode(documentNumber)}";

            return userRequestQueries;
        }

    }
}