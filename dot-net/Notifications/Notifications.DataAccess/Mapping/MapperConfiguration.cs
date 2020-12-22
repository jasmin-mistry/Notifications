namespace Notifications.DataAccess.Mapping
{
    public class MapperConfiguration
    {
        public static AutoMapper.MapperConfiguration Configure()
        {
            var config = new AutoMapper.MapperConfiguration(cfg => { cfg.AddProfile<NotificationsMapProfile>(); });

            config.AssertConfigurationIsValid();

            return config;
        }
    }
}