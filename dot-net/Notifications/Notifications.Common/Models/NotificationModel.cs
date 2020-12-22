﻿using System;
using Notifications.Common.Models.Enums;

namespace Notifications.Common.Models
{
    public class NotificationModel
    {
        public Guid Id { get; set; }

        public NotificationEventType EventType { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
    }
}