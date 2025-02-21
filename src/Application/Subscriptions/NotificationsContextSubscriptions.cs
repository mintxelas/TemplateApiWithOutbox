using Microsoft.Extensions.Logging;
using System;
using Sample.Domain;

namespace Sample.Application.Subscriptions;

public class NotificationsContextSubscriptions(ILogger<NotificationsContextSubscriptions> logger, IEventConsumer bus)
    : ISubscribeToContextEvents
{
    private readonly ILogger logger = logger;

    public void InitializeSubscriptions()
    {
        bus.Subscribe((MatchingMessageReceived @event) =>
        {
            logger.LogInformation("Found matching message with Id={id} at {instant:s}", @event.MessageId, DateTime.UtcNow);
        });
    }
}