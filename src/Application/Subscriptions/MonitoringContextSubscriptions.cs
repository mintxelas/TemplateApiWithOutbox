using Microsoft.Extensions.Logging;
using Sample.Domain;

namespace Sample.Application.Subscriptions
{
    public class MonitoringContextSubscriptions: ISubscribeToContextEvents
    {
        private readonly ILogger logger;
        private readonly IEventReader bus;

        public MonitoringContextSubscriptions(ILogger<MonitoringContextSubscriptions> logger, IEventReader bus)
        {
            this.logger = logger;
            this.bus = bus;
        }

        public void InitializeSubscriptions()
        {
            var matchesCounter = 0;
            bus.Subscribe((MatchingMessageReceived @event) =>
            {
                matchesCounter += 1;
                logger.LogInformation("Found matching message {matchesCounter} times", matchesCounter);
            });
        }
    }
}
