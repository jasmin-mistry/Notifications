using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Common.Models;
using Notifications.Common.Models.Enums;

namespace Notifications.Services.Tests
{
    public static class TestDataHelper
    {
        public static TemplateModel TemplateModel => new TemplateModel
        {
            Id = Guid.NewGuid(),
            EventType = NotificationEventType.AppointmentCancelled,
            Body =
                "Hi {FirstName}, your appointment with {OrganisationName} at {AppointmentDateTime} has been - cancelled for the following reason: {Reason}.",
            Title = "Appointment Cancelled"
        };

        public static IEnumerable<NotificationModel> GetNotifications(Guid userId, int count = 5)
        {
            return Enumerable.Range(0, count)
                .Select(x => GetNotification(userId));
        }

        public static NotificationModel GetNotification(Guid userId)
        {
            return new NotificationModel
            {
                UserId = userId,
                Id = Guid.NewGuid(),
                Body = $"TestBody-{userId}",
                EventType = NotificationEventType.AppointmentCancelled,
                Title = TemplateModel.Title
            };
        }
    }
}