namespace Sample.Application.ProcessMessage
{
    public class ProcessMessageResponse
    {
        public string Description { get; }

        public ProcessMessageResponse(string description)
        {
            Description = description;
        }
    }

    public class MessageToProcessNotFoundResponse : ProcessMessageResponse
    {
        public MessageToProcessNotFoundResponse(string description): base(description)
        {
            
        }
    }

    public class ErrorProcessingMessageResponse : ProcessMessageResponse
    {
        public ErrorProcessingMessageResponse(string error): base(error)
        {
            
        }
    }
}
