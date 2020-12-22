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
            if (eventModel == null) throw new InvalidEventModelException(nameof(eventModel));
            if (eventModel.EventType == null) throw new InvalidEventModelException(nameof(eventModel.EventType));
            if (eventModel.UserId == null || eventModel.UserId == Guid.Empty)
                throw new InvalidEventModelException(nameof(eventModel.UserId));
            if (eventModel.Data == null) throw new InvalidEventModelException(nameof(eventModel.Data));

            var template = await templatesAccess.Get(eventModel.EventType.GetValueOrDefault());

            if (template == null)
                throw new NotificationEventTypeNotSupportedException(eventModel.EventType.GetValueOrDefault());

            var notification = new NotificationModel
            {
                EventType = template.EventType,
                Title = template.Title,
                UserId = eventModel.UserId,
                Body = GetPopulatedBody(template.Body, eventModel.Data)
            };

            return await notificationsAccess.SaveNotification(notification);
        }

        private static string GetPopulatedBody(string bodyTemplate, IDataModel data)
        {
            var result = bodyTemplate;
            var properties = data.GetType().GetProperties();
            return properties.Aggregate(result,
                (current, property) => current.Replace($"{{{property.Name}}}", property.GetValue(data)?.ToString()));
        }
    }
}