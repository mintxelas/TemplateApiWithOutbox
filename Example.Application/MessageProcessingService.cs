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

        public virtual async Task Process(int messageId, string messageToMatch)
        {
            var message = await repository.GetById(messageId);
            message.Process(messageToMatch);
            await repository.Save(message);            
        }
    }
}
