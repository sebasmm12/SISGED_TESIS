using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Requests.Notifications;
using SISGED.Shared.Models.Responses.Notification;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;


        public NotificationsController(IMapper mapper, INotificationService notificationService)
        {
            _mapper = mapper;
            _notificationService = notificationService;
        }


        [HttpGet("{userId}")]
        public async Task<ActionResult<List<NotificationInfoResponse>>> GetNotificationsByUserIdAsync([FromRoute] string userId)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<NotificationUpdateResponse>> UpdateNotificationAsync([FromBody] NotificationUpdateRequest notificationUpdateRequest)
        {
            try
            {
                var notification = await _notificationService.GetNotificationAsync(notificationUpdateRequest.NotificationId);

                var updatedNotification = await _notificationService.UpdateNotificationAsync(notification);

                var updatedNotificationResponse = new NotificationUpdateResponse(updatedNotification.Id, updatedNotification.Seen);

                return Ok(updatedNotificationResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
