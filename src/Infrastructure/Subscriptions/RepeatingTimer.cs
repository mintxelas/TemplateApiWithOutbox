using System;
using System.Diagnostics;
using System.Threading;

namespace Template.Infrastructure.Subscriptions
{
    public sealed class RepeatingTimer: IDisposable
    {
        private readonly Timer timer;

        private bool processing;
        private bool Processing
        {
            get
            {
                lock (timer)
                {
                    return processing;
                }
            }

            set
            {
                lock (timer)
                {
                    processing = value;
                }
            }
        }

        public Action OnTick { get; set; }

        public RepeatingTimer(long dueTimeMilliseconds, long periodMilliseconds)
        {
            timer = new Timer(Tick, null, dueTimeMilliseconds, periodMilliseconds);
        }

        private void Tick(object state)
        {
            if (!Processing)
            {
                try
                {
                    Processing = true;
                    OnTick?.Invoke();
                }
                finally
                {
                    Processing = false;
                }
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
