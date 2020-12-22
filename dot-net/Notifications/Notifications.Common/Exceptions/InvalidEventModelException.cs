using System;

namespace Notifications.Common.Exceptions
{
    public class InvalidEventModelException : ArgumentNullException
    {
        public InvalidEventModelException(string paramName) :
            base($"'{paramName}' is missing in the payload.")
        {
        }
    }
}