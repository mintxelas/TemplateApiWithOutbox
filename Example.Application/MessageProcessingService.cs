using System.Dynamic;
using Example.Domain;
using System.Threading.Tasks;

namespace Example.Application
{
    public class MessageProcessingService
    {
        private readonly IMessageRepository repository;

        public MessageProcessingService(IMessageRepository repository)
        {
            this.repository = repository;
        }

        public virtual Task<int> Create(string text)
        {
            var message = new Message(text);
            return repository.Save(message);
        }

        public virtual async Task Process(int messageId, string messageToMatch)
        {
            var message = await repository.GetById(messageId);
            if (message is null) return;
            message.Process(messageToMatch);
            _ = await repository.Save(message);            
        }
    }
}
