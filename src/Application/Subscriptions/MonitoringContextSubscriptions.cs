using Microsoft.Extensions.Logging;
using Sample.Domain;

namespace Sample.Application.Subscriptions;

public class MonitoringContextSubscriptions(ILogger<MonitoringContextSubscriptions> logger, IEventConsumer bus)
    : ISubscribeToContextEvents
{
    private readonly ILogger logger = logger;

    public void InitializeSubscriptions()
    {
        var matchesCounter = 0;
        bus.Subscribe((MatchingMessageReceived @event) =>
        {
            matchesCounter += 1;
            logger.LogInformation("Found matching message {MatchedTimes} times", matchesCounter);
        });
    }
}