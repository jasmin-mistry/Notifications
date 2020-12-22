using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Notifications.Common.Exceptions;
using Notifications.Common.Interfaces;
using Notifications.Common.Models;
using Notifications.Common.Models.Enums;
using NUnit.Framework;
using Shouldly;

namespace Notifications.Services.Tests
{
    [TestFixture]
    public class NotificationsServiceTests
    {
        private MockRepository mockRepository;

        private Mock<INotificationsAccess> mockNotificationsAccess;
        private Mock<ITemplatesAccess> mockTemplatesAccess;

        private NotificationsService CreateService()
        {
            return new NotificationsService(
                mockNotificationsAccess.Object,
                mockTemplatesAccess.Object);
        }

        private static IEnumerable<TestCaseData> InvalidEventModelData
        {
            get
            {
                yield return new TestCaseData(null);
                yield return new TestCaseData(new EventModel
                {
                    EventType = null,
                    UserId = Guid.NewGuid(),
                    Data = new EventDataModel()
                });
                yield return new TestCaseData(new EventModel
                {
                    UserId = default,
                    EventType = NotificationEventType.AppointmentCancelled,
                    Data = new EventDataModel()
                });
                yield return new TestCaseData(new EventModel
                {
                    UserId = Guid.Empty,
                    EventType = NotificationEventType.AppointmentCancelled,
                    Data = new EventDataModel()
                });
                yield return new TestCaseData(new EventModel
                {
                    Data = null,
                    EventType = NotificationEventType.AppointmentCancelled,
                    UserId = Guid.NewGuid()
                });
            }
        }

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockNotificationsAccess = mockRepository.Create<INotificationsAccess>();
            mockTemplatesAccess = mockRepository.Create<ITemplatesAccess>();
        }

        [TearDown]
        public void TearDown()
        {
            mockRepository.VerifyAll();
        }

        [Test]
        public async Task CreateEventNotification_ShouldCreateNotificationInDatabase_WhenEventDataIsPassed()
        {
            var userId = Guid.NewGuid();
            var notification = TestDataHelper.GetNotification(userId);

            mockTemplatesAccess.Setup(x => x.Get(It.IsAny<NotificationEventType>()))
                .ReturnsAsync(TestDataHelper.TemplateModel);
            mockNotificationsAccess.Setup(x => x.SaveNotification(It.IsAny<NotificationModel>()))
                .ReturnsAsync(notification);

            var service = CreateService();
            var eventModel = new EventModel
            {
                UserId = Guid.NewGuid(),
                EventType = NotificationEventType.AppointmentCancelled,
                Data = new EventDataModel
                {
                    FirstName = "TestName",
                    OrganisationName = "TestOrg",
                    AppointmentDateTime = DateTime.Now,
                    Reason = "TestReason"
                }
            };

            var result = await service.CreateEventNotification(eventModel);

            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            mockNotificationsAccess.Verify(x =>
                x.SaveNotification(It.Is<NotificationModel>(model =>
                    model.UserId == eventModel.UserId &&
                    model.EventType == eventModel.EventType &&
                    model.Title == TestDataHelper.TemplateModel.Title &&
                    model.Body.Contains(eventModel.Data.FirstName) &&
                    model.Body.Contains(eventModel.Data.OrganisationName) &&
                    model.Body.Contains(eventModel.Data.Reason) &&
                    model.Body.Contains(eventModel.Data.AppointmentDateTime.ToString(CultureInfo.CurrentCulture))
                )));
        }

        [Test]
        [TestCaseSource(nameof(InvalidEventModelData))]
        public async Task
            CreateEventNotification_ShouldThrowInvalidEventModelException_WhenEventDataIsMissing(EventModel eventModel)
        {
            var service = CreateService();

            await Should.ThrowAsync<InvalidEventModelException>(service.CreateEventNotification(eventModel));

            mockTemplatesAccess.Verify(x => x.Get(It.IsAny<NotificationEventType>()), Times.Never);
            mockNotificationsAccess.Verify(x => x.SaveNotification(It.IsAny<NotificationModel>()), Times.Never);
        }

        [Test]
        public async Task
            CreateEventNotification_ShouldThrowNotificationEventTypeNotSupportedException_WhenEventTemplateIsNotSupported()
        {
            mockTemplatesAccess.Setup(x => x.Get(It.IsAny<NotificationEventType>()))
                .ReturnsAsync(default(TemplateModel));
            var service = CreateService();
            var eventModel = new EventModel
            {
                UserId = Guid.NewGuid(),
                EventType = NotificationEventType.AppointmentCancelled,
                Data = new EventDataModel()
            };

            await Should.ThrowAsync<NotificationEventTypeNotSupportedException>(
                service.CreateEventNotification(eventModel));

            mockNotificationsAccess.Verify(x => x.SaveNotification(It.IsAny<NotificationModel>()), Times.Never);
        }

        [Test]
        public void GetAllNotifications_StateUnderTest_ExpectedBehavior()
        {
            mockNotificationsAccess.Setup(x => x.GetAllNotifications()).Returns(new List<NotificationModel>());
            var service = CreateService();

            var result = service.GetAllNotifications();

            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<IReadOnlyCollection<NotificationModel>>();
        }

        [Test]
        public void GetUserNotifications_ShouldReturnAllNotifications_WhenUserIdIsPassed()
        {
            var userId = Guid.NewGuid();
            var userNotifications = TestDataHelper.GetNotifications(userId).ToList();
            mockNotificationsAccess.Setup(x => x.GetUserNotifications(userId)).Returns(userNotifications);
            var service = CreateService();

            var result = service.GetUserNotifications(userId);

            result.ShouldNotBeNull();
            result.Count.ShouldBe(userNotifications.Count);
        }

        [Test]
        public void GetUserNotifications_ShouldReturnNoRecord_WhenNoNotificationsAreAvailableForTheUser()
        {
            var userId = Guid.NewGuid();
            mockNotificationsAccess.Setup(x => x.GetUserNotifications(userId))
                .Returns(new List<NotificationModel>());
            var service = CreateService();

            var result = service.GetUserNotifications(userId);

            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }
    }
}