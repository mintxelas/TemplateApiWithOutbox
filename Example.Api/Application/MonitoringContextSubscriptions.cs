using Example.Api.Model;
using Microsoft.Extensions.Logging;

namespace Example.Api.Application
{
    public class MonitoringContextSubscriptions
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
