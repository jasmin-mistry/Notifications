using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<NotificationModel>))]
        public IReadOnlyCollection<NotificationModel> Get()
        {
            // TODO : Paging the result would be ideal when returning a bulk response
            return notificationsService.GetAllNotifications();
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<NotificationModel>))]
        public IReadOnlyCollection<NotificationModel> Get([FromRoute] Guid userId)
        {
            // TODO : Paging the result would be ideal when returning a bulk response
            return notificationsService.GetUserNotifications(userId);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NotificationModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] EventModel eventModel)
        {
            try
            {
                var notification = await notificationsService.CreateEventNotification(eventModel);
                return CreatedAtAction(nameof(Post), notification);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}