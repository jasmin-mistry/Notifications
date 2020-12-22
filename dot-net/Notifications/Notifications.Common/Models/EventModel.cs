using System;
using Notifications.Common.Models.Enums;

namespace Notifications.Common.Models
{
    public class EventModel
    {
        public NotificationEventType EventType { get; set; }
        public EventDataModel Data { get; set; }
        public Guid UserId { get; set; }
    }
}