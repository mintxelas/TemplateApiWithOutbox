using Example.Domain;
using System.Collections.Generic;

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

        public Message GetById(int id)
        {
            return database[id];
        }

        public void Save(Message message)
        {
            database[message.Id] = message;
        }
    }
}
