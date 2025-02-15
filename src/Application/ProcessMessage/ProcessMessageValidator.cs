using FluentValidation;

namespace Sample.Application.ProcessMessage
{
    public class ProcessMessageValidator: AbstractValidator<ProcessMessageRequest>
    {
        public ProcessMessageValidator()
        {
            RuleFor(x => x.MessageId).NotEqual(_ => default);
        }
    }
}
