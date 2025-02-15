using FluentValidation;

namespace Sample.Application.CreateMessage
{
    public class CreateMessageValidator : AbstractValidator<CreateMessageRequest>
    {
        public CreateMessageValidator()
        {
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}
