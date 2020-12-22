using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Notifications.Common.Models;
using Notifications.Common.Models.Enums;
using NUnit.Framework;
using Shouldly;

namespace Notifications.IntegrationTests.Controllers
{
    [TestFixture]
    public class NotificationsControllerTests : TestBase
    {
        private const string ApiNotifications = "/api/notifications/";

        private Task<HttpResponseMessage> GetNotifications()
        {
            return Get($"{ApiNotifications}");
        }

        private Task<HttpResponseMessage> GetNotifications(Guid userId)
        {
            return Get($"{ApiNotifications}{userId}");
        }

        private Task<HttpResponseMessage> AddNotification(dynamic eventData)
        {
            return PostJson(ApiNotifications, eventData);
        }

        [Test]
        public async Task Get_ShouldReturnNotifications_WhenUserIdIsPassed()
        {
            var userId = Guid.NewGuid();

            var response = await GetNotifications(userId);

            response.IsSuccessStatusCode.ShouldBeTrue();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public async Task Get_ShouldReturnOk_WhenApiCallIsSuccessful()
        {
            var response = await GetNotifications();

            response.IsSuccessStatusCode.ShouldBeTrue();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public async Task Post_ShouldReturnBadRequest_WhenEventTypeIsNotValid()
        {
            var eventData = new EventModel
            {
                EventType = NotificationEventType.AppointmentCancelled,
                UserId = Guid.NewGuid(),
                Data = new EventDataModel
                {
                    FirstName = "TestName",
                    OrganisationName = "TestOrg",
                    AppointmentDateTime = DateTime.Now,
                    Reason = "Appointment not needed anymore"
                }
            };

            var payload = BuildJsonApiPayload(eventData);
            payload = payload.Replace("\"EventType\":0", "\"EventType\":1"); // replacing to invalid event type 
            var response = await AddNotification(payload);

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrWhiteSpace();
            content.ShouldBe("'1' event type is not supported.");
        }

        [Test]
        public async Task Post_ShouldReturnBadRequest_WhenPayloadIsNotValid()
        {
            var response = await AddNotification("");

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Post_ShouldReturnOk_WhenNotificationIsCreated()
        {
            var eventData = new EventModel
            {
                EventType = NotificationEventType.AppointmentCancelled,
                UserId = Guid.NewGuid(),
                Data = new EventDataModel
                {
                    FirstName = "TestName",
                    OrganisationName = "TestOrg",
                    AppointmentDateTime = DateTime.Now,
                    Reason = "Appointment not needed anymore"
                }
            };
            var response = await AddNotification(BuildJsonApiPayload(eventData));

            response.IsSuccessStatusCode.ShouldBeTrue();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var notification = JsonConvert.DeserializeObject<NotificationModel>(content);

            notification.UserId.ShouldBe(eventData.UserId);
            notification.Id.ShouldNotBe(Guid.Empty);
            notification.Body.ShouldContain(eventData.Data.FirstName);
            notification.Body.ShouldContain(eventData.Data.OrganisationName);
            notification.Body.ShouldContain(eventData.Data.Reason);
        }
    }
}