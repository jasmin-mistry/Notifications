using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Notifications.Common.Interfaces;
using Notifications.Common.Models;
using Notifications.Common.Models.Enums;

namespace Notifications.DataAccess.Access
{
    public class TemplatesesAccess : ITemplatesAccess
    {
        private readonly NotificationsDbContext dbContext;
        private readonly IMapper mapper;

        public TemplatesesAccess(NotificationsDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<TemplateModel> Get(NotificationEventType notificationEventType)
        {
            var result = await dbContext.Templates.FirstOrDefaultAsync(x => x.EventType == notificationEventType);

            return result != null ? mapper.Map<TemplateModel>(result) : default;
        }
    }
}