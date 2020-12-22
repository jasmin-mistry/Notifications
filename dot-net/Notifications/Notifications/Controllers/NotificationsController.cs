using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Notifications.Common.Exceptions;
using Notifications.Common.Interfaces;
using Notifications.Common.Models;

namespace Notifications.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            this.notificationsService = notificationsService;
        }

        [Route("")]
        [HttpGet]
        public IReadOnlyCollection<NotificationModel> Get()
        {
            return notificationsService.GetAllNotifications();
        }

        [HttpGet("{userId}")]
        public IReadOnlyCollection<NotificationModel> Get([FromRoute] Guid userId)
        {
            return notificationsService.GetUserNotifications(userId);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventModel eventModel)
        {
            try
            {
                var notification = await notificationsService.CreateEventNotification(eventModel);
                return Ok(notification);
            }
            catch (NotificationEventTypeNotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}