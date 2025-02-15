using System;

namespace Sample.Infrastructure.Configuration
{
    public class OutBoxConfiguration : IValidateConfiguration
    {
        public string Database { get; set; }
        public TimerConfiguration Timer { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Database))
                throw new InvalidCastException("Please specify a database connectionString");

            if (Timer is null)
                throw new InvalidCastException("Timer section is mandatory");
        }
    }

    public class TimerConfiguration : IValidateConfiguration
    {
        public int DueSeconds { get; set; }
        public int PeriodSeconds { get; set; }

        public void Validate()
        {
            if (DueSeconds < -1)
                throw new IndexOutOfRangeException($"{nameof(DueSeconds)} must be -1 or above.");

            if (PeriodSeconds < -1)
                throw new IndexOutOfRangeException($"{nameof(PeriodSeconds)} must be -1 or above.");
        }
    }
}
