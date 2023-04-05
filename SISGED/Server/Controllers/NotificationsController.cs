using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SISGED.Server.Hubs;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
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
        private readonly IUserService _userService;
        private readonly ITemplateService _templateService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub, INotificationHub> _notificationHub;

        public NotificationsController(IMapper mapper, INotificationService notificationService, 
                IUserService userService, ITemplateService templateService, IRoleService roleService, IHubContext<NotificationHub, INotificationHub> notificationHub)
        {
            _mapper = mapper;
            _notificationService = notificationService;
            _userService = userService;
            _templateService = templateService;
            _roleService = roleService;
            _notificationHub = notificationHub;
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

        [HttpPost("users")]
        public async Task<ActionResult> RegisterNotificationFromUserToUserAsync([FromBody] NotificationRegisterRequest notificationRegisterRequest)
        {
            try
            {
                var notification = _mapper.Map<Notification>(notificationRegisterRequest);

                var notificationUsers = await GetNotificationUsersAsync(notificationRegisterRequest.SenderUserId, notificationRegisterRequest.ReceiverUserId);

                var senderUserRole = await _roleService.GetRoleByIdAsync(notificationUsers.SenderUser.Rol);

                var userNotification = new UserNotificationDTO(notificationUsers.SenderUser.GetFullName(), senderUserRole.Label, notificationRegisterRequest.Document.Title);

                var templateFilterDto = new TemplateFilterDTO(notificationUsers.SenderUser.Type, notificationUsers.ReceiverUser.Type,
                                                 notificationRegisterRequest.ActionId, notificationRegisterRequest.Type);


                var inserted = await RegisterNotificationAsync(userNotification, templateFilterDto, notification);

                await _notificationHub.Clients.All.RecieveNotificationAsync(inserted.ReceiverId);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #region private methods
        private async Task<Notification> RegisterNotificationAsync<T>(T notificationModel, TemplateFilterDTO templateFilterDTO, Notification notification)
        {
            var notificationTemplate = await _templateService.GetTemplateAsync(templateFilterDTO);

            SetNotificationDescription(notificationTemplate, notificationModel);

            _mapper.Map(notificationTemplate, notification);

            return await _notificationService.RegisterNotificationAsync(notification);
        }

        private static void SetNotificationDescription<T>(Template notificationTemplate, T notificationModel)
        {
            var notificationModelProperties = notificationModel!
                                                    .GetType()
                                                    .GetProperties()
                                                    .Where(property => property.CanRead && property.CanWrite)
                                                    .ToList();

            foreach (var property in notificationModelProperties)
            {
                notificationTemplate.Description = notificationTemplate
                                                                .Description
                                                                .Replace("{" + property.Name + "}", 
                                                                            (string)property.GetValue(notificationModel, null)!);
            }    
            
        }

        private async Task<Template> GetTemplateAsync(TemplateFilterDTO templateFilterDTO)
        {
            //var templateFilterDTO = new TemplateFilterDTO(notificationUserDTO.SenderUser.Type, notificationUserDTO.ReceiverUser.Type, 
            //                                    notificationRegisterRequest.ActionId, notificationRegisterRequest.Type);

            return await _templateService.GetTemplateAsync(templateFilterDTO);
        }

        private async Task<NotificationUsersDTO> GetNotificationUsersAsync(string senderUserId, string receiverUserId)
        {
            var senderUserTask = _userService.GetUserByIdAsync(senderUserId);
            var receiverUserTask = _userService.GetUserByIdAsync(receiverUserId);

            await Task.WhenAll(senderUserTask, receiverUserTask);

            var senderUser = await senderUserTask;
            var receiverUser = await receiverUserTask;

            return new(senderUser, receiverUser);
        }
        #endregion
    }
}
