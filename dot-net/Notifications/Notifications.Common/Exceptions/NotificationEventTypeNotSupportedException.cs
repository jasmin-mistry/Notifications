using System;
using Notifications.Common.Models.Enums;

namespace Notifications.Common.Exceptions
{
    public class NotificationEventTypeNotSupportedException : Exception
    {
        public NotificationEventTypeNotSupportedException(NotificationEventType eventModelEventType) :
            base($"'{eventModelEventType}' event type is not supported.")
        {
        }
    }
}