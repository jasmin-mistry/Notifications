using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notifications.DataAccess;

namespace Notifications.IntegrationTests
{
    public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public NotificationsDbContext DbContext;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var pathToWeb = SolutionPathUtility.GetProjectPath(@"Notifications");

            builder
                .UseSolutionRelativeContentRoot(AppContext.BaseDirectory.Replace($".{Constants.Environment}", ""))
                .ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile(Path.Combine(pathToWeb,
                        $"{Constants.AppsettingsFileName}.{Constants.AppsettingsFileExtension}"));
                    conf.AddJsonFile(Path.Combine(pathToWeb,
                        $"{Constants.AppsettingsFileName}.{Constants.Environment}.{Constants.AppsettingsFileExtension}"));
                })
                .ConfigureServices(services =>
                {
                    var descriptor =
                        services.SingleOrDefault(d =>
                            d.ServiceType == typeof(DbContextOptions<NotificationsDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<NotificationsDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(new Guid().ToString());
                    });

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        DbContext = scopedServices.GetRequiredService<NotificationsDbContext>();

                        var logger = scopedServices
                            .GetRequiredService<ILogger<ApiWebApplicationFactory<TStartup>>>();

                        DbContext.Database.EnsureCreated();

                        try
                        {
                            DbContext.Notifications.AddRange(TestDataHelper.GetNotifications(Guid.NewGuid()));
                            DbContext.Templates.Add(TestDataHelper.TemplateEntity);
                            DbContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "An error occurred seeding the " +
                                                $"database with test messages. Error: {ex.Message}");
                        }
                    }
                });

            base.ConfigureWebHost(builder);
        }
    }
}