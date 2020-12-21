using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Common.Interfaces;
using Notifications.Common.Models;

namespace Notifications.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsAccess notificationsAccess;

        public NotificationsService(INotificationsAccess notificationsAccess)
        {
            this.notificationsAccess = notificationsAccess;
        }

        public IReadOnlyCollection<NotificationModel> GetAllNotifications()
        {
            return this.notificationsAccess.GetAllNotifications().ToList();
        }
    }
}
