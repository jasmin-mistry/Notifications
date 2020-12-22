using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notifications.Common.Models;

namespace Notifications.Common.Interfaces
{
    public interface INotificationsService
    {
        IReadOnlyCollection<NotificationModel> GetAllNotifications();
        IReadOnlyCollection<NotificationModel> GetUserNotifications(Guid userId);
        Task<NotificationModel> CreateEventNotification(EventModel eventModel);
    }
}