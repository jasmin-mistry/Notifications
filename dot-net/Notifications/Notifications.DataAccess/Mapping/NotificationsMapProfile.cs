using AutoMapper;
using Notifications.Common.Models;
using Notifications.DataAccess.Entities;

namespace Notifications.DataAccess.Mapping
{
    public class NotificationsMapProfile : Profile
    {
        public NotificationsMapProfile()
        {
            CreateMap<NotificationEntity, NotificationModel>().ReverseMap();
            CreateMap<TemplateEntity, TemplateModel>().ReverseMap();
        }
    }
}