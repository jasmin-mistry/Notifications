using System;
using Notifications.Common.Interfaces;

namespace Notifications.Common.Models
{
    public class EventDataModel : IDataModel
    {
        public string FirstName { get; set; }
        public string OrganisationName { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Reason { get; set; }
    }
}