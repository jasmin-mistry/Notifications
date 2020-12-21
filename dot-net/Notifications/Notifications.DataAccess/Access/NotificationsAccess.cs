using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notifications.Common.Interfaces;
using Notifications.Common.Models;
using Notifications.DataAccess.Entities;

namespace Notifications.DataAccess.Access
{
    public class NotificationsAccess : INotificationsAccess
    {
        private readonly NotificationsDbContext dbContext;

        public NotificationsAccess(NotificationsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<NotificationModel> GetAllNotifications()
        {
            return dbContext.Notifications.Select(x => new NotificationModel()
            {
                Id = x.Id,
            });
        }
    }
}
