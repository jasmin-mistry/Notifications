using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notifications.Common.Models;

namespace Notifications.Common.Interfaces
{
    public interface INotificationsAccess
    {
        IEnumerable<NotificationModel> GetAllNotifications();
        IEnumerable<NotificationModel> GetUserNotifications(Guid userId);
        Task<NotificationModel> SaveNotification(NotificationModel notificationModel);
    }
}