using Microsoft.AspNetCore.Components;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Requests.Notifications;
using SISGED.Shared.Models.Responses.Notification;

namespace SISGED.Client.Shared
{
    public partial class Notification
    {
        [Inject]
        public IMomentRepository MomentRepository { get; set; } = default!;
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public NotificationStrategy NotificationStrategy { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public NotificationInfoResponse NotificationInfo { get; set; } = default!;
        [Parameter]
        public string NotificationState { get; set; } = default!;
        [Parameter]
        public EventCallback<NotificationUpdateResponse> NotificationUpdate { get; set; }

        private string notificationTimeFromNow = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            notificationTimeFromNow = await MomentRepository.GetTimeFromNowAsync(NotificationInfo.IssueDate);
        }

        private void NavigateTo()
        {
            NavigationManager.NavigateTo(NotificationInfo.Link);
        }

        private async Task UpdateNotificationStateAsync(string notificationId)
        {
            NotificationState = string.Empty;

            var notification = await UpdateNotificationAsync(new(notificationId));

            if(notification is null)
            {
                NotificationState = NotificationStrategy.GetNotificationStatus(NotificationInfo.Seen);

                return;
            }

            await NotificationUpdate.InvokeAsync(notification);
        }

        private async Task<NotificationUpdateResponse?> UpdateNotificationAsync(NotificationUpdateRequest notificationUpdateRequest)
        {
            try
            {
                var notificationResponse = await HttpRepository.PutAsync<NotificationUpdateRequest, NotificationUpdateResponse>($"api/notifications", notificationUpdateRequest);

                if (notificationResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo actualizar el estado de la notificación");
                }

                return notificationResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo actualizar el estado de la notificación");

                return null;
            }
        }
    }
}