namespace Sample.Application.CreateMessage;

public class CreateMessageRequest(string text) 
{
    public string Text { get; } = text;
}