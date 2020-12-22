using System;
using System.Linq;
using Notifications.Common.Interfaces;
using Notifications.Common.Models.Enums;

namespace Notifications.Common.Models
{
    public class TemplateModel
    {
        public Guid Id { get; set; }
        public NotificationEventType EventType { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }

        public string BodyText(IDataModel data)
        {
            var result = Body;
            var properties = data.GetType().GetProperties();
            return properties.Aggregate(result,
                (current, property) => current.Replace($"{{{property.Name}}}", property.GetValue(data)?.ToString()));
        }
    }
}