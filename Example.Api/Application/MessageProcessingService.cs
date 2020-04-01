using Example.Api.Model;

namespace Example.Api.Application
{
    public class MessageProcessingService
    {
        private const string MessageToMatch = "Hello";
        private readonly IMessageRepository repository;

        public MessageProcessingService(IMessageRepository repository)
        {
            this.repository = repository;
        }

        public void Process(int messageId)
        {
            var message = repository.GetById(messageId);
            message.Process(MessageToMatch);
            repository.Save(message);
        }
    }
}
