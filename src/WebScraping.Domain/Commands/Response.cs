namespace WebScraping.Domain.Commands
{
    using prmToolkit.NotificationPattern;
    using System.Collections.Generic;

    public class Response
    {
        public IEnumerable<Notification> Notifications { get; }
        public bool Success { get; private set; }
        public object Data { get; private set; }

        public Response(INotifiable notifiable)
        {
            this.Success = notifiable.IsValid();
            this.Notifications = notifiable.Notifications;

        }

        public Response(INotifiable notifiable, object data)
        {
            this.Success = notifiable.IsValid();
            this.Notifications = notifiable.Notifications;
            this.Data = data;

        }
    }

}
