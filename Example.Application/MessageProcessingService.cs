using Example.Model;

namespace Example.Application
{
    public class MessageProcessingService
    {
        private readonly IMessageRepository repository;

        public MessageProcessingService(IMessageRepository repository)
        {
            this.repository = repository;
        }

        public void Process(int messageId, string messageToMatch)
        {
            var message = repository.GetById(messageId);
            message.Process(messageToMatch);
            repository.Save(message);
        }
    }
}
