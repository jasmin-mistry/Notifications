using System.Threading.Tasks;
using Notifications.Common.Models;
using Notifications.Common.Models.Enums;

namespace Notifications.Common.Interfaces
{
    public interface ITemplatesAccess
    {
        Task<TemplateModel> Get(NotificationEventType notificationEventType);
    }
}