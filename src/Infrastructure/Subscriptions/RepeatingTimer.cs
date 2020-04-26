using System;
using System.Threading;

namespace Template.Infrastructure.Subscriptions
{
    public sealed class RepeatingTimer: IDisposable
    {
        private readonly Timer timer;

        public Action OnTick { get; set; }

        public RepeatingTimer(long dueTimeMilliseconds, long periodMilliseconds)
        {
            timer = new Timer(Tick, null, dueTimeMilliseconds, periodMilliseconds);
        }

        private void Tick(object state)
        {
            OnTick?.Invoke();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
