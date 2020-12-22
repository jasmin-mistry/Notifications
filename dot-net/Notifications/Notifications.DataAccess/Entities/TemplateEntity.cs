using System;
using System.ComponentModel.DataAnnotations;
using Notifications.Common.Models.Enums;

namespace Notifications.DataAccess.Entities
{
    public class TemplateEntity
    {
        [Key] public Guid Id { get; set; }

        public NotificationEventType EventType { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
    }
}