using System.Diagnostics;
using System.Threading;
using Template.Infrastructure.Subscriptions;
using Xunit;

namespace Template.Infrastructure.Tests
{
    public class RepeatingTimerShould
    {
        [Fact]
        public void invoke_handler_for_first_time_when_dueTime_passes()
        {
            var invokedTime = new Stopwatch();
            invokedTime.Start();
            var invocations = 0;
            var timer = new RepeatingTimer(10, -1)
            {
                OnTick = () => invocations += 1
            };

            while (invocations < 1)
                Thread.Sleep(5);

            invokedTime.Stop();
            timer.Dispose();
            Assert.True(invokedTime.ElapsedMilliseconds>=10);
        }

        [Fact]
        public void invoke_handler_every_period_passes_Starting_immediately()
        {
            var invokedTime = new Stopwatch();
            invokedTime.Start();
            var invocations = 0;
            var timer = new RepeatingTimer(0, 10)
            {
                OnTick = () => invocations += 1
            };
            
            while(invocations < 3)
                Thread.Sleep(5);

            timer.Dispose();
            invokedTime.Stop();
            Assert.True(invokedTime.ElapsedMilliseconds >= 20);
        }
    }
}
