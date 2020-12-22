using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Notifications.Common.Interfaces;
using Notifications.Common.Models;
using Notifications.DataAccess.Entities;

namespace Notifications.DataAccess.Access
{
    public class NotificationsAccess : INotificationsAccess
    {
        private readonly NotificationsDbContext dbContext;
        private readonly IMapper mapper;

        public NotificationsAccess(NotificationsDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public IEnumerable<NotificationModel> GetUserNotifications(Guid userId)
        {
            var result = dbContext.Notifications
                .Where(notification => notification.UserId == userId);

            return result.Select(x => mapper.Map<NotificationModel>(x));
        }

        public async Task<NotificationModel> SaveNotification(NotificationModel notificationModel)
        {
            var notificationEntity = mapper.Map<NotificationEntity>(notificationModel);

            // assuming that notifications will always be created and not updated.
            await dbContext.Notifications.AddAsync(notificationEntity);

            await dbContext.SaveChangesAsync();

            return mapper.Map<NotificationModel>(notificationEntity);
        }

        public IEnumerable<NotificationModel> GetAllNotifications()
        {
            var result = dbContext.Notifications;

            return result.Select(x => mapper.Map<NotificationModel>(x));
        }
    }
}