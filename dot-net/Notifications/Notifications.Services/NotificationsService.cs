using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Notifications.Common.Exceptions;
using Notifications.Common.Interfaces;
using Notifications.Common.Models;

namespace Notifications.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsAccess notificationsAccess;
        private readonly ITemplatesAccess templatesAccess;

        public NotificationsService(INotificationsAccess notificationsAccess, ITemplatesAccess templatesAccess)
        {
            this.notificationsAccess = notificationsAccess;
            this.templatesAccess = templatesAccess;
        }

        public IReadOnlyCollection<NotificationModel> GetAllNotifications()
        {
            var result = notificationsAccess.GetAllNotifications().ToList();

            return new ReadOnlyCollection<NotificationModel>(result);
        }

        public IReadOnlyCollection<NotificationModel> GetUserNotifications(Guid userId)
        {
            var notifications = notificationsAccess.GetUserNotifications(userId).ToList();

            return new ReadOnlyCollection<NotificationModel>(notifications);
        }

        public async Task<NotificationModel> CreateEventNotification(EventModel eventModel)
        {
            var template = await templatesAccess.Get(eventModel.EventType);

            if (template == null) throw new NotificationEventTypeNotSupportedException(eventModel.EventType);

            var notification = new NotificationModel
            {
                EventType = template.EventType,
                Title = template.Title,
                UserId = eventModel.UserId,
                Body = template.BodyText(eventModel.Data)
            };

            return await notificationsAccess.SaveNotification(notification);
        }
    }
}