using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Common.Models.Enums;
using Notifications.DataAccess.Entities;

namespace Notifications.IntegrationTests
{
    public static class TestDataHelper
    {
        public static TemplateEntity TemplateEntity => new TemplateEntity
        {
            Id = Guid.NewGuid(),
            EventType = NotificationEventType.AppointmentCancelled,
            Body =
                "Hi {FirstName}, your appointment with {OrganisationName} at {AppointmentDateTime} has been - cancelled for the following reason: {Reason}.",
            Title = "Appointment Cancelled"
        };

        public static IEnumerable<NotificationEntity> GetNotifications(Guid userId, int count = 5)
        {
            return Enumerable.Range(0, count)
                .Select(x => GetNotification(userId));
        }

        public static NotificationEntity GetNotification(Guid userId)
        {
            return new NotificationEntity
            {
                UserId = userId,
                Id = Guid.NewGuid(),
                Body = $"TestBody-{userId}",
                EventType = NotificationEventType.AppointmentCancelled,
                Title = TemplateEntity.Title
            };
        }
    }
}