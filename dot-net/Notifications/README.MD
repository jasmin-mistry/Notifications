# Notifications Microservice - Scenario

The Notifications microservice manages user notifications. It responds to system wide events and depending on the type of event it will generate a new user notification and store it in its store. 

When a user logs in, the user will fetch all of their notifications. A socket is opened between the notifications service and the user’s browser, which allows the notifications service to push new notifications to a user during their session.

## Appointment cancelled notification 

### Requirements 
The notification service listens for the ‘Appointment Cancelled’ event and on receiving the event the service will create a new notification model and store it in the notifications database. Event data includes:
- FirstName 
- AppointmentDateTime
- OrganisationName 
- Reason

The notification service stores a predefined notification template associated to the Appointment Cancelled event. On generating a new notification, the notificaiton template is retrieved from the store, it’s notification body is interpolated with the event data, and stored with the user id.
The notification service can return back notifications for a given user id.

### Instructions:
Create a table that stores the notification template with the following data:
- Id: guid
- EventType: AppointmentCancelled
- Body: ‘Hi {Firstname}, your appointment with {OrganisationName} at {AppointmentDateTime} has been - cancelled for the following reason: {Reason}.’
- Title: ‘Appointment Cancelled’

Store the notification template - this can be done directly using Sql Management studio

Create a way for the notification service to receive events. This can be through a HTTP Post. Events should have the following shape:
- Type
- Data
- UserId

Handle the event:
- Read template from store based on EventType
- Create new User Notification by combining the template with the data received in the Event.
- Store the User notification

Create an API to fetch notifications by user id. 

If you have time, add the ability for a user to establish a socket connection that sends new notifications as they're recieved.

Include examples of Unit Tests and Integration tests where possible.


