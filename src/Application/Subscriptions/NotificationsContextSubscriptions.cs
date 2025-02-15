using Microsoft.Extensions.Logging;
using System;
using Sample.Domain;

namespace Sample.Application.Subscriptions
{
    public class NotificationsContextSubscriptions: ISubscribeToContextEvents
    {
        private readonly ILogger logger;
        private readonly IEventReader bus;

        public NotificationsContextSubscriptions(ILogger<NotificationsContextSubscriptions> logger, IEventReader bus)
        {
            this.logger = logger;
            this.bus = bus;
        }

        public void InitializeSubscriptions()
        {
            bus.Subscribe((MatchingMessageReceived @event) =>
            {
                logger.LogInformation("Found matching message with Id={id} at {instant:s}", @event.MessageId, DateTime.UtcNow);
            });
        }
    }
}
