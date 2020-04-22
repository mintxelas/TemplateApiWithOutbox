﻿using Example.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Example.Infrastructure
{
    public class MessageRepository : IMessageRepository
    {
        private readonly Dictionary<int, Message> database = new Dictionary<int, Message> { 
            [0] = new Message(0, "Hello"), 
            [1] = new Message(1, "One"), 
            [2] = new Message(2, "Two"), 
            [3] = new Message(3, "Three") 
        };
        private readonly IEventWriter bus;

        public MessageRepository(IEventWriter bus) => this.bus = bus;

        public Task<Message> GetById(int id) 
            => Task.FromResult(database[id]);

        public Task Save(Message message)
        {
            database[message.Id] = message;
            var withEvents = (IExposeEvents)message;
            foreach (var @event in withEvents.PendingEvents)
            {
                bus.Publish(@event);
            }
            withEvents.ClearPendingEvents();
            return Task.CompletedTask;
        }
    }
}
