using System;

namespace Example.Infrastructure.Entities
{
    public class OutboxEvent
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? ProcessedDate { get; set; }
        public string EventName { get; set; }
        public string Payload { get; set; }
    }
}
