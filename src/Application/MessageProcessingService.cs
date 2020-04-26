using System;
using System.Threading.Tasks;
using Template.Domain;

namespace Template.Application
{
    public class MessageProcessingService
    {
        private readonly IMessageRepository repository;

        public MessageProcessingService(IMessageRepository repository)
        {
            this.repository = repository;
        }

        public virtual Task<Message> Create(string text)
        {
            var message = new Message(text);
            return repository.Save(message);
        }

        public virtual async Task Process(Guid messageId, string messageToMatch)
        {
            var message = await repository.GetById(messageId);
            if (message is null) return;
            message.Process(messageToMatch);
            await repository.Save(message);            
        }
    }
}
