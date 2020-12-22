using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        private static IEnumerable<TestCaseData> InvalidEventModelData
        {
            get
            {
                yield return new TestCaseData(default(EventModel));
                yield return new TestCaseData(new EventModel {UserId = default, Data = new EventDataModel()});
                yield return new TestCaseData(new EventModel {UserId = Guid.Empty, Data = new EventDataModel()});
                yield return new TestCaseData(new EventModel {UserId = Guid.NewGuid(), Data = null});
            }
        }

        private static void VerifyNotificationObject(NotificationModel notification, Guid userId, EventModel eventData)
        {
            notification.ShouldNotBeNull();
            notification.UserId.ShouldBe(userId);
            notification.Id.ShouldNotBe(Guid.Empty);
            notification.Body.ShouldContain(eventData.Data.FirstName);
            notification.Body.ShouldContain(eventData.Data.OrganisationName);
            notification.Body.ShouldContain(eventData.Data.Reason);
            notification.Body.ShouldContain(eventData.Data.AppointmentDateTime.ToString(CultureInfo.CurrentCulture));
        }

        [Test, Order(2)]
        public async Task Get_ShouldReturnNotifications_WhenUserIdIsPassed()
        {
            var userId = Guid.NewGuid();
            var eventData = new EventModel
            {
                EventType = NotificationEventType.AppointmentCancelled,
                UserId = userId,
                Data = new EventDataModel
                {
                    FirstName = "TestName",
                    OrganisationName = "TestOrg",
                    AppointmentDateTime = DateTime.Now,
                    Reason = "Appointment not needed anymore"
                }
            };
            await AddNotification(
                BuildJsonApiPayload(eventData)); // create a notification to allow GetNotifications to return data

            var response = await GetNotifications(userId);

            response.IsSuccessStatusCode.ShouldBeTrue();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var notifications = JsonConvert.DeserializeObject<List<NotificationModel>>(content);
            notifications.ShouldNotBeNull();
            notifications.Count.ShouldBeGreaterThan(0);
            VerifyNotificationObject(notifications.FirstOrDefault(), userId, eventData);
        }

        [Test]
        public async Task Get_ShouldReturnOk_WhenApiCallIsSuccessful()
        {
            var response = await GetNotifications();

            response.IsSuccessStatusCode.ShouldBeTrue();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test, Order(1)]
        public async Task Post_ShouldReturn201Created_WhenNotificationIsCreated()
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
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var content = await response.Content.ReadAsStringAsync();
            var notification = JsonConvert.DeserializeObject<NotificationModel>(content);
            VerifyNotificationObject(notification, eventData.UserId, eventData);
        }

        [Test]
        public async Task Post_ShouldReturnBadRequest_WhenEventTypeIsNotValid()
        {
            var eventData = new EventModel
            {
                EventType = null,
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
            var response = await AddNotification(payload);

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrWhiteSpace();
            content.ShouldBe("Value cannot be null. (Parameter ''EventType' is missing in the payload.')");
        }

        [Test]
        [TestCaseSource(nameof(InvalidEventModelData))]
        public async Task Post_ShouldReturnBadRequest_WhenPayloadIsNotValid(EventModel eventData)
        {
            var response = await AddNotification(BuildJsonApiPayload(eventData));

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}