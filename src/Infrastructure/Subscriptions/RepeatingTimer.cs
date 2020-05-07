using System;
using System.Threading;
using Template.Infrastructure.Configuration;

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

        public RepeatingTimer(TimerConfiguration configuration)
        {
            var due = configuration.DueSeconds > 0 ? configuration.DueSeconds * 1000 : configuration.DueSeconds;
            var period = configuration.PeriodSeconds > 0 ? configuration.PeriodSeconds * 1000 : configuration.PeriodSeconds;
            timer = new Timer(Tick, null, due, period);
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
