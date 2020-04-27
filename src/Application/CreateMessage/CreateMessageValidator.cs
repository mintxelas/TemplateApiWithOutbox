using System.Runtime.InteropServices.ComTypes;
using FluentValidation;

namespace Template.Application.CreateMessage
{
    public class CreateMessageValidator : AbstractValidator<CreateMessageRequest>
    {
        public CreateMessageValidator()
        {
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}
