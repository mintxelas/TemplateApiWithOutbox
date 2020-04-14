using Example.Model;
using System.Linq;
using Xunit;

namespace Example.Domain.Tests
{
    public class MessageShould
    {
        private const int SomeMessageId = 123456;
        private const string SomeExpectedText = "given text";
        private const string SomeUnexpectedText = "unexpected text";

        [Fact]
        public void publish_an_event_when_message_text_and_searched_text_match()
        {
            var expectedEvent = new MatchingMessageReceived(SomeMessageId);
            var message = new Message(SomeMessageId, SomeExpectedText);

            message.Process(SomeExpectedText);

            var withEvents = (IExposeEvents)message;
            Assert.Contains(expectedEvent.MessageId, withEvents.PendingEvents.Select(e => ((MatchingMessageReceived)e).MessageId));
        }

        [Fact]
        public void not_publish_any_event_when_message_text_and_searched_text_do_not_match()
        {
            var message = new Message(SomeMessageId, SomeUnexpectedText);

            message.Process(SomeExpectedText);

            var withEvents = (IExposeEvents)message;
            Assert.Empty(withEvents.PendingEvents);
        }
    }
}
