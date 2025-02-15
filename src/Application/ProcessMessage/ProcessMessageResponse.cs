namespace Sample.Application.ProcessMessage;

public class ProcessMessageResponse(string description)
{
    public string Description { get; } = description;
}

public class MessageToProcessNotFoundResponse(string description) : ProcessMessageResponse(description);

public class ErrorProcessingMessageResponse(string error) : ProcessMessageResponse(error);