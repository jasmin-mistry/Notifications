using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Notifications.Common.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NotificationEventType
    {
        [EnumMember(Value = "AppointmentCancelled")]
        AppointmentCancelled = 0
    }
}