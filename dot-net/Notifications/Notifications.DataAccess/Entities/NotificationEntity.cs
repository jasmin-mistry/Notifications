﻿using System;
using Notifications.Common.Models.Enums;

namespace Notifications.DataAccess.Entities
{
    public class NotificationEntity
    {
        public Guid Id { get; set; }

        public NotificationEventType EventType { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
    }
}